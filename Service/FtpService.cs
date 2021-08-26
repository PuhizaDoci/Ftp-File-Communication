using FtpFileCommunication.Extensions;
using FtpFileCommunication.Models;
using NLog;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace FtpFileCommunication.Service
{
    public class FtpService
    {
        private OrdersStructure OrdersStructure { get; set; }
        private OrdersStructureHeader OrderHeader { get; set; }
        private OrdersStructureLine OrderLine { get; set; }
        private string FtpHost { get; set; }
        private string FtpPort { get; set; }
        private string FtpPassword { get; set; }
        private string FtpUsername { get; set; }
        private string xmlToWrite { get; set; } = "";

        private readonly static FtpConfigGroup _ftpConfig =
            ConfigurationManager.GetSection("ftpConfigGroup") as FtpConfigGroup;

        private readonly string rootPathOut = 
            AppDomain.CurrentDomain.BaseDirectory + @"OUT\";
        private readonly string rootPathDone =
            AppDomain.CurrentDomain.BaseDirectory + @"FtpSent\";

        private static Logger Logger { get; set; }

        public FtpService()
        {
            Logger = LogManager.GetCurrentClassLogger();
            //Ftp settings from web.config
            FtpHost = _ftpConfig.FtpConfig[0].Value;
            FtpPort = _ftpConfig.FtpConfig[1].Value;
            FtpPassword = _ftpConfig.FtpConfig[2].Value;
            FtpUsername = _ftpConfig.FtpConfig[3].Value;

            Logger.Info("Vlerat nga web.config jane: " + FtpHost + "/ "
                + FtpPort + "/ " + FtpUsername + "/ " + FtpPassword);
        }

        public void SendToFtp()
        {
            Logger.Info("Fillimi i krijimit te struktures se klasave");

            //Structure of classes based on the Xml format provided
            try
            {
                OrdersStructure = new OrdersStructure();
                OrderHeader = OrdersStructure.ordersStructureHeader;
                OrderLine = new OrdersStructureLine();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error: Deshtim i krijimit te struktures se klasave" + ex.Message);
            }

            Logger.Info("Perfundimi i krijimit te struktures se klasave");

            try
            {
                // All the process is in this method
                FillHeaderClass();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void FillHeaderClass()
        {
            //Find all the orders in the database (conditions are in the procedure)
            Logger.Info("Fillimi i marrjes se porosive ne server");
            DataTable dtHeader;

            try
            {
                dtHeader = DalClass.GetOrders(null);
                Logger.Info("Perfundim i marrjes se porosive ne server." +
                    " Numri i rreshtave eshte: " + dtHeader.Rows.Count);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error: Deshtim i marrjes se" +
                    " porosive ne server: " + ex.Message);
                throw ex;
            }

            //Iterate each document found
            foreach (DataRow dataRow in dtHeader.Rows)
            {
                Logger.Info("Fillim i dergimit te porosise me id: " 
                    + dataRow["PurchaseOrderNo"].ToString());

                OrderHeader.PurchaseOrderNo = dataRow["PurchaseOrderNo"].ToString();
                OrderHeader.PurchaseOrderDate = Convert.ToDateTime(
                    dataRow["PurchaseOrderDate"]).ToString("dd/MM/yyyy");
                if (dataRow["RequestedDeliveryDate"] != DBNull.Value)
                    OrderHeader.RequestedDeliveryDate = Convert.ToDateTime(
                        dataRow["RequestedDeliveryDate"]).ToString("dd/MM/yyyy");
                OrderHeader.BuyerGLN = dataRow["BuyerGLN"].ToString();
                OrderHeader.BuyerName = dataRow["BuyerName"].ToString();
                OrderHeader.ShipToGLN = dataRow["ShipToGLN"].ToString();
                OrderHeader.ShipToName = dataRow["ShipToName"].ToString();

                if (dataRow["ShipToAddress"] != DBNull.Value 
                    || dataRow["ShipToAddress"].ToString() != " ")
                    OrderHeader.ShipToAddress = dataRow["ShipToAddress"].ToString();

                if (dataRow["ShipToAddress"] != DBNull.Value 
                    || dataRow["ShipToAddress"].ToString() != " ")
                    OrderHeader.ShipToCity = dataRow["ShipToCity"].ToString();

                if (dataRow["ShipToAddress"] != DBNull.Value 
                    || dataRow["ShipToAddress"].ToString() != " ")
                    OrderHeader.ShipToPhone = dataRow["ShipToPhone"].ToString();

                if (dataRow["ShipToAddress"] != DBNull.Value 
                    || dataRow["ShipToAddress"].ToString() != " ")
                    OrderHeader.ShipToMail = dataRow["ShipToMail"].ToString();

                OrderHeader.SellerGLN = dataRow["SellerGLN"].ToString();
                OrderHeader.SellerName = dataRow["SellerName"].ToString();
                OrderHeader.ShipFromGLN = dataRow["ShipFromGLN"].ToString();
                OrderHeader.ShipFromName = dataRow["ShipFromName"].ToString();

                Logger.Info("Fillim i dergimit te lines per porosine me id: "
                    + dataRow["PurchaseOrderNo"].ToString());

                try
                {
                    //Find the Lines
                    FillLineClass(OrderHeader.PurchaseOrderNo);
                    Logger.Info("Perfundim i mbushjes se detaje per porosine me id: " 
                        + dataRow["PurchaseOrderNo"].ToString());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error: Deshtim i marrjes se detajeve" +
                        " ne server: " + ex.Message);
                    throw ex;
                }

                try
                {
                    //Serialize the classes to Xml formats
                    SerializeToXml();
                    Logger.Info("Perfundim i serializimit ne xml");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error: Deshtim i serializimit ne xml: " + ex.Message);
                    throw ex;
                }

                string fileName = "";
                string path = "";

                try
                {
                    //Create and save the file in the rootPathOut directory
                    fileName = "ORD_" + OrderHeader.BuyerGLN + 
                        "_" + OrderHeader.PurchaseOrderNo + ".xml";

                    Logger.Info("Krijimi dhe ruajtja e file. Emri i file: " + fileName);
                    path = rootPathOut + fileName;
                    SaveFile(path, xmlToWrite);

                    Logger.Info("Krijimi dhe ruajtja e file u perfundua");

                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error: Deshtim i ruajtjes ose krijimit te file: "
                        + ex.Message);
                    throw ex;
                }

                try
                {
                    // Send the created file to the FtpSite (Settings are saved in web.config)
                    await UploadFilesService.UploadFileAsync(
                        path, fileName, FtpHost, 
                        Convert.ToInt32(FtpPort), FtpUsername, FtpPassword)
                        .ConfigureAwait(false);

                    Logger.Info("Dergimi i file ne ftp u perfundua");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error: Deshtim i ruajtjes se file ne FTP "
                        + ex.Message);
                    throw ex;
                }

                //If sent successfully move local file to "FtpSent" directory
                MoveFiles(path, fileName);

                //Update status in db
                DalClass.UpdateOrderFtpStatus(
                    Convert.ToInt64(OrderHeader.PurchaseOrderNo), true);
            }
        }

        private void FillLineClass(string documentId)
        {
            DataTable dtLines = new DataTable();
            bool isParsed = long.TryParse(documentId, out long intDocumentId);

            //Get Lines from database based on documentId
            if (isParsed)
                dtLines = DalClass.GetOrdersLines(null, intDocumentId);
            else
                throw new Exception("Error parsing string!",
                    new Exception("DocumentId as string could not be parsed to int64"));


            Logger.Info("Marrja e detaleve perfundoi, numri i rreshtave eshte: " + dtLines.Rows.Count);

            //Iterate lines, fill the class
            if (dtLines.Rows.Count > 0)
            {
                OrderHeader.OrderLine = new OrdersStructureLine[dtLines.Rows.Count];

                Logger.Info("U krijua struktura e klases OrderLine.");

                int i = 0;
                foreach (DataRow dataRow in dtLines.Rows)
                {
                    OrderLine = new OrdersStructureLine();
                    OrderLine.LineNo = Convert.ToInt32(dataRow["LineNo"]);
                    OrderLine.BuyerItemCode = dataRow["BuyerItemCode"].ToString();
                    OrderLine.SellerItemCode = dataRow["SellerItemCode"].ToString();
                    OrderLine.GTIN = dataRow["GTIN"].ToString();
                    OrderLine.ItemName = dataRow["ItemName"].ToString();
                    OrderLine.UnitOfMeasure = dataRow["UnitOfMeasure"].ToString();
                    OrderLine.OrderedQuantity = Convert.ToDecimal(dataRow["OrderedQuantity"]);
                    OrderLine.OrderedPackets = Convert.ToDecimal(dataRow["OrderedPackets"]);
                    OrderLine.ExpectedPriceWithoutVat = Convert.ToDecimal(dataRow["ExpectedPriceWithoutVat"]);
                    OrderLine.VatValue = Convert.ToDecimal(dataRow["VatValue"]);
                    OrderHeader.OrderLine[i] = OrderLine;

                    i++;
                }
            }
        }

        private void SerializeToXml()
        {
            //Serialize a xml based on OrdersStructure class
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(typeof(OrdersStructure));
            System.Xml.Serialization.XmlSerializerNamespaces ns = 
                new System.Xml.Serialization.XmlSerializerNamespaces();
            xmlToWrite = "";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            settings.Encoding = new UTF8Encoding(false);
            ns.Add("", "");

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, OrdersStructure, ns);
                }
                xmlToWrite = textWriter.ToString(); //This is the output as a string
                if (xmlToWrite.Contains("utf-16"))
                    xmlToWrite = xmlToWrite.Replace("utf-16", "utf-8");
            }
        }

        private void SaveFile(string path, string text)
        {
            // Check for the directory, create if not exists
            if (!Directory.Exists(rootPathOut))
                Directory.CreateDirectory(rootPathOut);

            // Create the file
            if (!File.Exists(path))
                File.Create(path).Dispose();

            //Write the xml content to the file
            if (File.Exists(path))
            {
                try
                {
                    using TextWriter tw = new StreamWriter(path);
                    tw.WriteLine(text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void MoveFiles(string oldPath, string fileName)
        {
            if (!Directory.Exists(rootPathDone))
                Directory.CreateDirectory(rootPathDone);

            if (oldPath != null && File.Exists(oldPath))
            {
                File.Move(oldPath, rootPathDone + fileName);
                File.Delete(oldPath);
            }
        }
    }
}