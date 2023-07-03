using Common.Lang;
using Common.Lang.Entity;

namespace Common.Api.Billing
{
    public class SkuInfo : AbstractEntityIdString
    {
        /// <summary>
        /// price (formatted, i.e. $1.85)
        /// </summary>
        public string priceFormatted;
        
        /// <summary>
        /// price amount
        /// </summary>
        public float price;
        
        /// <summary>
        /// ISO 4217 currency code for price. 
        /// For example, if price is specified in British pounds sterling, 
        /// price_currency_code is "GBP".
        /// </summary>
        public string priceCurrencyCode;
        
        /// <summary>
        /// item title
        /// </summary>
        public string title;
        
        /// <summary>
        /// item description
        /// </summary>
        public string description;
        
        /// <summary>
        /// SKU type.
        /// Consumable, NonConsumable, Subscription
        /// </summary>
        public SkuType type;

        public override string ToString()
        {
            return "SkuInfo: id=" + Id +
                   ", priceText=" + priceFormatted +
                   ", price=" + price +
                   ", priceCurrencyCode=" + priceCurrencyCode +
                   ", title=" + title +
                   ", description=" + description +
                   ", type=" + type;
        }
    }
}
