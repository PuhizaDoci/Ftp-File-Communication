using System;
using System.Xml.Serialization;

namespace FtpFileCommunication.Models
{
    [XmlRoot("SSBEdi_orderMessage")]
    public class OrdersStructure
    {
        [XmlElement("order")]
        public OrdersStructureHeader ordersStructureHeader = new OrdersStructureHeader();
    }

    public class OrdersStructureHeader
    {
        //Order number (mandatory)
        [XmlElement("PurchaseOrderNo")]
        public string PurchaseOrderNo { get; set; }

        //Order date (mandatory)
        [XmlElement("PurchaseOrderDate")]
        public string PurchaseOrderDate { get; set; }

        //Delivery date, if exists (optional)
        [XmlElement("RequestedDeliveryDate")]
        public string? RequestedDeliveryDate { get; set; } = null;

        //Buyer Id/Code (mandatory)
        [XmlElement("BuyerGLN")]
        public string BuyerGLN { get; set; }

        //Buyer Name (mandatory)
        [XmlElement("BuyerName")]
        public string BuyerName { get; set; }

        //Id of company where the goods are delivered (mandatory)
        [XmlElement("ShipToGLN")]
        public string ShipToGLN { get; set; }

        //Name of the company to deliver (mandatory)
        [XmlElement("ShipToName")]
        public string ShipToName { get; set; }

        //Address of the company to deliver (optional)
        [XmlElement("ShipToAddress")]
        public string ShipToAddress { get; set; }

        //City of the company to deliver (optional)
        [XmlElement("ShipToCity")]
        public string ShipToCity { get; set; }

        //Phone number of the company where the goods are delivered (optional)
        [XmlElement("ShipToPhone")]
        public string ShipToPhone { get; set; }

        //E-mail address of the company where the goods are delivered (optional)
        [XmlElement("ShipToeMail")]
        public string ShipToMail { get; set; }

        //Id of the seller (mandatory)
        [XmlElement("SellerGLN")]
        public string SellerGLN { get; set; }

        //Name of the seller (mandatory)
        [XmlElement("SellerName")]
        public string SellerName { get; set; }

        //Code of subject where the goods are being shipped from (optional)
        [XmlElement("ShipFromGLN")]
        public string ShipFromGLN { get; set; }

        //Name of subject where the goods are being shipped from (optional)
        [XmlElement("ShipFromName")]
        public string ShipFromName { get; set; }

        [XmlElement("OrderLine")]
        public OrdersStructureLine[] OrderLine { get; set; }
    }

    public class OrdersStructureLine
    {
        //Unique line number (mandatory)
        [XmlElement("LineNo")]
        public int LineNo { get; set; }

        //Item code (optional)
        [XmlElement("BuyerItemCode")]
        public string BuyerItemCode { get; set; }

        //Item code at vendor (opional)
        [XmlElement("SellerItemCode")]
        public string SellerItemCode { get; set; }

        //Item barcode (mandatory)
        [XmlElement("GTIN")]
        public string GTIN { get; set; }

        //Item name (mandatory)
        [XmlElement("ItemName")]
        public string ItemName { get; set; }

        //Unit of measure (mandatory)
        [XmlElement("UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        //Order quantity (mandatory) 3 decimal places
        [XmlElement("OrderedQuantity")]
        public decimal OrderedQuantity { get; set; }

        //Order quantity in packages (optional) 3 decimal places
        [XmlElement("OrderedPackets")]
        public decimal OrderedPackets { get; set; }

        //Price without VAT (optional) 4 decimal places
        [XmlElement("ExpectedPriceWithoutVat")]
        public decimal ExpectedPriceWithoutVat { get; set; }

        //Vat in percentage (optional) 2 decimal places
        [XmlElement("VatValue")]
        public decimal VatValue { get; set; }
    }
}