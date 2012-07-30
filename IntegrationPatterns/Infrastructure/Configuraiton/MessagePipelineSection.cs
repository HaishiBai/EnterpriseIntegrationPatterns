using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace IntegrationPatterns.Infrastructure.Configuraiton
{
    public class MessagePipelineSection: ConfigurationSection
    {
        [ConfigurationProperty("pipelines", IsDefaultCollection=true)]
        [ConfigurationCollection(typeof(PipelinesCollection), AddItemName="add", ClearItemsName="clear", RemoveItemName="remvoe")]
        public PipelinesCollection Pipelines
        {
            get
            {
                PipelinesCollection collection = (PipelinesCollection)base["pipelines"];
                return collection;
            }
        }
    }
    public class PipelinesCollection: ConfigurationElementCollection
    {
        public PipelinesCollection()
        {           
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new PipelineElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PipelineElement)element).Name;
        }
        public PipelineElement this[int index]
        {
            get
            {
                return (PipelineElement)BaseGet(index);
            }
        }
        public void Add(PipelineElement pipeline)
        {
            BaseAdd(pipeline);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false); //false -> don't throw if exists. TODO: is this desired behavior?
        }
        public void Remove(PipelineElement pipeline)
        {
            if (BaseIndexOf(pipeline) >= 0)
                BaseRemove(pipeline.Name);
        }
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        public void Clear()
        {
            BaseClear();
        }
    }
    public class PipelineElement : ConfigurationElement
    {
        [ConfigurationProperty("channels", IsDefaultCollection=false)]
        [ConfigurationCollection(typeof(ChannelsCollection), AddItemName="add", ClearItemsName="clear", RemoveItemName="remove")]
        public ChannelsCollection Channels
        {
            get
            {
                ChannelsCollection collection = (ChannelsCollection)base["channels"];
                return collection;
            }
        }

        [ConfigurationProperty("units", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ProcessingUnitsCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ProcessingUnitsCollection ProcessingUnits
        {
            get
            {
                ProcessingUnitsCollection collection = (ProcessingUnitsCollection)base["units"];
                return collection;
            }
        }

        [ConfigurationProperty("name", IsRequired=true, IsKey=true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }
    public class ChannelsCollection : ConfigurationElementCollection
    {
        public ChannelsCollection()
        {
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChannelElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ChannelElement)element).Name;
        }
        public ChannelElement this[int index]
        {
            get
            {
                return (ChannelElement)BaseGet(index);
            }
        }
        public void Add(ChannelElement channel)
        {
            BaseAdd(channel);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false); //false -> don't throw if exists. TODO: is this desired behavior?
        }
        public void Remove(ChannelElement channel)
        {
            if (BaseIndexOf(channel) >= 0)
                BaseRemove(channel.Name);
        }
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        public void Clear()
        {
            BaseClear();
        }
    }
    public class ChannelElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
        [ConfigurationProperty("type", IsRequired = true)]
        public ChannelType ChannelType
        {
            get
            {
                return (ChannelType)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
        [ConfigurationProperty("scheme", IsRequired=true, DefaultValue="sb")]
        public string Scheme
        {
            get
            {
                return (string)this["scheme"];
            }
            set
            {
                this["scheme"] = value;
            }
        }
        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get
            {
                return (string)this["connectionString"];
            }
            set
            {
                this["connectionString"] = value;
            }
        }
    }
    public class ProcessingUnitsCollection : ConfigurationElementCollection
    {
        public ProcessingUnitsCollection()
        {
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProcessingUnitElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProcessingUnitElement)element).Name;
        }
      
        public ProcessingUnitElement this[int index]
        {
            get
            {
                return (ProcessingUnitElement)BaseGet(index);
            }
        }
        public void Add(ProcessingUnitElement unit)
        {
            BaseAdd(unit);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false); //false -> don't throw if exists. TODO: is this desired behavior?
        }
        public void Remove(ProcessingUnitElement unit)
        {
            if (BaseIndexOf(unit) >= 0)
                BaseRemove(unit.Name);
        }
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        public void Clear()
        {
            BaseClear();
        }
    }
    public class ProcessingUnitElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
        [ConfigurationProperty("type", IsRequired = true)]
        public string UnitType
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
        [ConfigurationProperty("inputs")]
        [ConfigurationCollection(typeof(ChannelReferencesCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remvoe")]
        public ChannelReferencesCollection Inputs
        {
            get
            {
                ChannelReferencesCollection collection = (ChannelReferencesCollection)base["inputs"];
                return collection;
            }
        }
        [ConfigurationProperty("outputs")]
        [ConfigurationCollection(typeof(ChannelReferencesCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remvoe")]
        public ChannelReferencesCollection Outputs
        {
            get
            {
                ChannelReferencesCollection collection = (ChannelReferencesCollection)base["outputs"];
                return collection;
            }
        }
        [ConfigurationProperty("controls")]
        [ConfigurationCollection(typeof(ChannelReferencesCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remvoe")]
        public ChannelReferencesCollection Controls
        {
            get
            {
                ChannelReferencesCollection collection = (ChannelReferencesCollection)base["controls"];
                return collection;
            }
        }
    }
    public enum ChannelType
    {
        input = 1,
        output = 2,
        control = 3
    }
    public class ChannelReferencesCollection : ConfigurationElementCollection
    {
        public ChannelReferencesCollection()
        {
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChannelReferenceElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ChannelReferenceElement)element).Name;
        }
        public ChannelReferenceElement this[int index]
        {
            get
            {
                return (ChannelReferenceElement)BaseGet(index);
            }
        }
        public void Add(ChannelReferenceElement channel)
        {
            BaseAdd(channel);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false); //false -> don't throw if exists. TODO: is this desired behavior?
        }
        public void Remove(ChannelReferenceElement channel)
        {
            if (BaseIndexOf(channel) >= 0)
                BaseRemove(channel.Name);
        }
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        public void Clear()
        {
            BaseClear();
        }
    }
    public class ChannelReferenceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }
}
