/*
 * File: PaymentGatewayOptions.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Configuration options for payment gateway integration
 * 
 * Usage: Holds payment gateway configuration parameters.
 *        Loaded from appsettings.json via IOptions pattern.
 *        Supports multiple gateway configurations (Stripe, PayPal, Square, etc).
 *        Can be extended when payment license is obtained.
 * 
 * Dependencies: None (configuration POCO)
 */

namespace CIMarkets.Blockchain.Infrastructure.ExternalApis
{
    /// <summary>
    /// Configuration options for payment gateway integration.
    /// </summary>
    public class PaymentGatewayOptions
    {
        /// <summary>
        /// Payment gateway provider name (e.g., Stripe, PayPal, Square).
        /// </summary>
        public string Provider { get; set; } = "Mock";

        /// <summary>
        /// Payment gateway API endpoint URL.
        /// </summary>
        public string ApiUrl { get; set; } = "https://api.payment.example.com";

        /// <summary>
        /// API key for authentication with payment gateway.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Secret key for secure communication with payment gateway.
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Public key for client-side payment forms.
        /// </summary>
        public string? PublicKey { get; set; }

        /// <summary>
        /// Webhook URL for receiving payment notifications.
        /// </summary>
        public string? WebhookUrl { get; set; }

        /// <summary>
        /// Webhook secret for verifying incoming notifications.
        /// </summary>
        public string? WebhookSecret { get; set; }

        /// <summary>
        /// Request timeout in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Whether to use sandbox/test environment.
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// Supported currencies for payments.
        /// </summary>
        public string[] SupportedCurrencies { get; set; } = { "USD", "EUR", "BTC", "ETH" };

        /// <summary>
        /// Supported payment methods.
        /// </summary>
        public string[] SupportedMethods { get; set; } = { "card", "wallet", "crypto" };

        /// <summary>
        /// Enable payment logging for debugging.
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// Enable payment request validation.
        /// </summary>
        public bool ValidateRequests { get; set; } = true;

        /// <summary>
        /// Optional custom parameters for specific gateways.
        /// </summary>
        public Dictionary<string, string> CustomParameters { get; set; } = new();
    }
}
