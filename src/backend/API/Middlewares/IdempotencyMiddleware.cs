using System.Security.Cryptography;
using System.Text;

namespace API.Middlewares
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;

        public IdempotencyMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // read the idempotency key
            var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (String.IsNullOrEmpty(key))
            {
                await _next(context);
                return;
            }

            // Step 0: Compute request hash
            var requestHash = await ComputeRequestHashAsync(context);

            context.Items["IdempotencyKey"] = key;
            context.Items["RequestHash"] = requestHash;

            await _next(context);
        }

        private async static Task<string> ComputeRequestHashAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true
            );

            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            var rawRequest = $"{context.Request.Method}:{context.Request.Path}:{body}";
            var bytes = Encoding.UTF8.GetBytes(rawRequest);

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(bytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
