using System.Configuration;

namespace FtpFileCommunication.Extensions
{
    public class FtpConfigGroup : ConfigurationSection
    {
        [ConfigurationProperty("ftpSettings")]
        public FtpConfigGroupElementCollection FtpConfig
        {
            get { return (FtpConfigGroupElementCollection)this["ftpSettings"]; }
        }
    }

    [ConfigurationCollection(typeof(FtpGroupElement))]
    public class FtpConfigGroupElementCollection : ConfigurationElementCollection
    {
        public FtpGroupElement this[int index]
        {
            get { return (FtpGroupElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new FtpGroupElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FtpGroupElement)element).Name;
        }
    }

    public class FtpGroupElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}