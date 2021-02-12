//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// This code was generated by XmlSchemaClassGenerator version 2.0.502.0.
namespace Erabikata.Backend.Models.Input.Generated
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("JMdict", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("JMdict", Namespace="http://www.w3.org/namespace/")]
    public partial class JMdict
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Entry> _entry;
        
        [System.Xml.Serialization.XmlElementAttribute("entry")]
        public System.Collections.ObjectModel.Collection<Entry> Entry
        {
            get
            {
                return this._entry;
            }
            private set
            {
                this._entry = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Entry collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EntrySpecified
        {
            get
            {
                return (this.Entry.Count != 0);
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="JMdict" /> class.</para>
        /// </summary>
        public JMdict()
        {
            this._entry = new System.Collections.ObjectModel.Collection<Entry>();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("entry", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("entry", Namespace="http://www.w3.org/namespace/")]
    public partial class Entry
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("ent_seq")]
        public Ent_Seq Ent_Seq { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<K_Ele> _k_Ele;
        
        [System.Xml.Serialization.XmlElementAttribute("k_ele")]
        public System.Collections.ObjectModel.Collection<K_Ele> K_Ele
        {
            get
            {
                return this._k_Ele;
            }
            private set
            {
                this._k_Ele = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the K_Ele collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool K_EleSpecified
        {
            get
            {
                return (this.K_Ele.Count != 0);
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="Entry" /> class.</para>
        /// </summary>
        public Entry()
        {
            this._k_Ele = new System.Collections.ObjectModel.Collection<K_Ele>();
            this._r_Ele = new System.Collections.ObjectModel.Collection<R_Ele>();
            this._sense = new System.Collections.ObjectModel.Collection<Sense>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<R_Ele> _r_Ele;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("r_ele")]
        public System.Collections.ObjectModel.Collection<R_Ele> R_Ele
        {
            get
            {
                return this._r_Ele;
            }
            private set
            {
                this._r_Ele = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Sense> _sense;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("sense")]
        public System.Collections.ObjectModel.Collection<Sense> Sense
        {
            get
            {
                return this._sense;
            }
            private set
            {
                this._sense = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("ent_seq", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("ent_seq", Namespace="http://www.w3.org/namespace/")]
    public partial class Ent_Seq
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("k_ele", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("k_ele", Namespace="http://www.w3.org/namespace/")]
    public partial class K_Ele
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("keb")]
        public Keb Keb { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Ke_Inf> _ke_Inf;
        
        [System.Xml.Serialization.XmlElementAttribute("ke_inf")]
        public System.Collections.ObjectModel.Collection<Ke_Inf> Ke_Inf
        {
            get
            {
                return this._ke_Inf;
            }
            private set
            {
                this._ke_Inf = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Ke_Inf collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Ke_InfSpecified
        {
            get
            {
                return (this.Ke_Inf.Count != 0);
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="K_Ele" /> class.</para>
        /// </summary>
        public K_Ele()
        {
            this._ke_Inf = new System.Collections.ObjectModel.Collection<Ke_Inf>();
            this._ke_Pri = new System.Collections.ObjectModel.Collection<Ke_Pri>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Ke_Pri> _ke_Pri;
        
        [System.Xml.Serialization.XmlElementAttribute("ke_pri")]
        public System.Collections.ObjectModel.Collection<Ke_Pri> Ke_Pri
        {
            get
            {
                return this._ke_Pri;
            }
            private set
            {
                this._ke_Pri = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Ke_Pri collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Ke_PriSpecified
        {
            get
            {
                return (this.Ke_Pri.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("keb", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("keb", Namespace="http://www.w3.org/namespace/")]
    public partial class Keb
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("ke_inf", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("ke_inf", Namespace="http://www.w3.org/namespace/")]
    public partial class Ke_Inf
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("ke_pri", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("ke_pri", Namespace="http://www.w3.org/namespace/")]
    public partial class Ke_Pri
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("r_ele", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("r_ele", Namespace="http://www.w3.org/namespace/")]
    public partial class R_Ele
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("reb")]
        public Reb Reb { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("re_nokanji")]
        public Re_Nokanji Re_Nokanji { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Re_Restr> _re_Restr;
        
        [System.Xml.Serialization.XmlElementAttribute("re_restr")]
        public System.Collections.ObjectModel.Collection<Re_Restr> Re_Restr
        {
            get
            {
                return this._re_Restr;
            }
            private set
            {
                this._re_Restr = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Re_Restr collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Re_RestrSpecified
        {
            get
            {
                return (this.Re_Restr.Count != 0);
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="R_Ele" /> class.</para>
        /// </summary>
        public R_Ele()
        {
            this._re_Restr = new System.Collections.ObjectModel.Collection<Re_Restr>();
            this._re_Inf = new System.Collections.ObjectModel.Collection<Re_Inf>();
            this._re_Pri = new System.Collections.ObjectModel.Collection<Re_Pri>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Re_Inf> _re_Inf;
        
        [System.Xml.Serialization.XmlElementAttribute("re_inf")]
        public System.Collections.ObjectModel.Collection<Re_Inf> Re_Inf
        {
            get
            {
                return this._re_Inf;
            }
            private set
            {
                this._re_Inf = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Re_Inf collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Re_InfSpecified
        {
            get
            {
                return (this.Re_Inf.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Re_Pri> _re_Pri;
        
        [System.Xml.Serialization.XmlElementAttribute("re_pri")]
        public System.Collections.ObjectModel.Collection<Re_Pri> Re_Pri
        {
            get
            {
                return this._re_Pri;
            }
            private set
            {
                this._re_Pri = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Re_Pri collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Re_PriSpecified
        {
            get
            {
                return (this.Re_Pri.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("reb", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("reb", Namespace="http://www.w3.org/namespace/")]
    public partial class Reb
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("re_nokanji", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("re_nokanji", Namespace="http://www.w3.org/namespace/")]
    public partial class Re_Nokanji
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("re_restr", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("re_restr", Namespace="http://www.w3.org/namespace/")]
    public partial class Re_Restr
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("re_inf", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("re_inf", Namespace="http://www.w3.org/namespace/")]
    public partial class Re_Inf
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("re_pri", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("re_pri", Namespace="http://www.w3.org/namespace/")]
    public partial class Re_Pri
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("sense", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("sense", Namespace="http://www.w3.org/namespace/")]
    public partial class Sense
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Stagk> _stagk;
        
        [System.Xml.Serialization.XmlElementAttribute("stagk")]
        public System.Collections.ObjectModel.Collection<Stagk> Stagk
        {
            get
            {
                return this._stagk;
            }
            private set
            {
                this._stagk = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Stagk collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StagkSpecified
        {
            get
            {
                return (this.Stagk.Count != 0);
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="Sense" /> class.</para>
        /// </summary>
        public Sense()
        {
            this._stagk = new System.Collections.ObjectModel.Collection<Stagk>();
            this._stagr = new System.Collections.ObjectModel.Collection<Stagr>();
            this._pos = new System.Collections.ObjectModel.Collection<Pos>();
            this._xref = new System.Collections.ObjectModel.Collection<Xref>();
            this._ant = new System.Collections.ObjectModel.Collection<Ant>();
            this._field = new System.Collections.ObjectModel.Collection<Field>();
            this._misc = new System.Collections.ObjectModel.Collection<Misc>();
            this._s_Inf = new System.Collections.ObjectModel.Collection<S_Inf>();
            this._lsource = new System.Collections.ObjectModel.Collection<Lsource>();
            this._dial = new System.Collections.ObjectModel.Collection<Dial>();
            this._gloss = new System.Collections.ObjectModel.Collection<Gloss>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Stagr> _stagr;
        
        [System.Xml.Serialization.XmlElementAttribute("stagr")]
        public System.Collections.ObjectModel.Collection<Stagr> Stagr
        {
            get
            {
                return this._stagr;
            }
            private set
            {
                this._stagr = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Stagr collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StagrSpecified
        {
            get
            {
                return (this.Stagr.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Pos> _pos;
        
        [System.Xml.Serialization.XmlElementAttribute("pos")]
        public System.Collections.ObjectModel.Collection<Pos> Pos
        {
            get
            {
                return this._pos;
            }
            private set
            {
                this._pos = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Pos collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PosSpecified
        {
            get
            {
                return (this.Pos.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Xref> _xref;
        
        [System.Xml.Serialization.XmlElementAttribute("xref")]
        public System.Collections.ObjectModel.Collection<Xref> Xref
        {
            get
            {
                return this._xref;
            }
            private set
            {
                this._xref = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Xref collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool XrefSpecified
        {
            get
            {
                return (this.Xref.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Ant> _ant;
        
        [System.Xml.Serialization.XmlElementAttribute("ant")]
        public System.Collections.ObjectModel.Collection<Ant> Ant
        {
            get
            {
                return this._ant;
            }
            private set
            {
                this._ant = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Ant collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AntSpecified
        {
            get
            {
                return (this.Ant.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Field> _field;
        
        [System.Xml.Serialization.XmlElementAttribute("field")]
        public System.Collections.ObjectModel.Collection<Field> Field
        {
            get
            {
                return this._field;
            }
            private set
            {
                this._field = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Field collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FieldSpecified
        {
            get
            {
                return (this.Field.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Misc> _misc;
        
        [System.Xml.Serialization.XmlElementAttribute("misc")]
        public System.Collections.ObjectModel.Collection<Misc> Misc
        {
            get
            {
                return this._misc;
            }
            private set
            {
                this._misc = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Misc collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MiscSpecified
        {
            get
            {
                return (this.Misc.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<S_Inf> _s_Inf;
        
        [System.Xml.Serialization.XmlElementAttribute("s_inf")]
        public System.Collections.ObjectModel.Collection<S_Inf> S_Inf
        {
            get
            {
                return this._s_Inf;
            }
            private set
            {
                this._s_Inf = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the S_Inf collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool S_InfSpecified
        {
            get
            {
                return (this.S_Inf.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Lsource> _lsource;
        
        [System.Xml.Serialization.XmlElementAttribute("lsource")]
        public System.Collections.ObjectModel.Collection<Lsource> Lsource
        {
            get
            {
                return this._lsource;
            }
            private set
            {
                this._lsource = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Lsource collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LsourceSpecified
        {
            get
            {
                return (this.Lsource.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Dial> _dial;
        
        [System.Xml.Serialization.XmlElementAttribute("dial")]
        public System.Collections.ObjectModel.Collection<Dial> Dial
        {
            get
            {
                return this._dial;
            }
            private set
            {
                this._dial = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Dial collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DialSpecified
        {
            get
            {
                return (this.Dial.Count != 0);
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Gloss> _gloss;
        
        [System.Xml.Serialization.XmlElementAttribute("gloss")]
        public System.Collections.ObjectModel.Collection<Gloss> Gloss
        {
            get
            {
                return this._gloss;
            }
            private set
            {
                this._gloss = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Gloss collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GlossSpecified
        {
            get
            {
                return (this.Gloss.Count != 0);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("stagk", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("stagk", Namespace="http://www.w3.org/namespace/")]
    public partial class Stagk
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("stagr", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("stagr", Namespace="http://www.w3.org/namespace/")]
    public partial class Stagr
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("pos", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("pos", Namespace="http://www.w3.org/namespace/")]
    public partial class Pos
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("xref", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("xref", Namespace="http://www.w3.org/namespace/")]
    public partial class Xref
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("ant", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("ant", Namespace="http://www.w3.org/namespace/")]
    public partial class Ant
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("field", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("field", Namespace="http://www.w3.org/namespace/")]
    public partial class Field
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("misc", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("misc", Namespace="http://www.w3.org/namespace/")]
    public partial class Misc
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("s_inf", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("s_inf", Namespace="http://www.w3.org/namespace/")]
    public partial class S_Inf
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("lsource", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("lsource", Namespace="http://www.w3.org/namespace/")]
    public partial class Lsource
    {
        
        [System.Xml.Serialization.XmlAttributeAttribute("ls_wasei")]
        public string Ls_Wasei { get; set; }
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("dial", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("dial", Namespace="http://www.w3.org/namespace/")]
    public partial class Dial
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("gloss", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("gloss", Namespace="http://www.w3.org/namespace/")]
    public partial class Gloss
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.ObjectModel.Collection<Pri> _pri;
        
        [System.Xml.Serialization.XmlElementAttribute("pri")]
        public System.Collections.ObjectModel.Collection<Pri> Pri
        {
            get
            {
                return this._pri;
            }
            private set
            {
                this._pri = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Pri collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PriSpecified
        {
            get
            {
                return (this.Pri.Count != 0);
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="Gloss" /> class.</para>
        /// </summary>
        public Gloss()
        {
            this._pri = new System.Collections.ObjectModel.Collection<Pri>();
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute("g_type")]
        public string G_Type { get; set; }
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.502.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("pri", Namespace="http://www.w3.org/namespace/", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("pri", Namespace="http://www.w3.org/namespace/")]
    public partial class Pri
    {
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text { get; set; }
    }
}
