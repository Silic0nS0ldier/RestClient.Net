﻿using System.Collections.Generic;

namespace RestClientNetSamples
{

    public class CUSTOMERList : List<CUSTOMER>
    {

    }

    public class CUSTOMER
    {
        public int ID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string STREET { get; set; }
        public string CITY { get; set; }
    }
}
