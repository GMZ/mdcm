// mDCM: A C# DICOM library
//
// Copyright (c) 2006-2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)

using System;
using System.IO;
using System.Xml.Serialization;

namespace Dicom.Utility
{
    [Serializable]
    public class XmlSerializable<T>
    {
        public void Save(string filename)
        {
            var serializer = new XmlSerializer(GetType());
            using (var fs = File.Create(filename))
            {
                serializer.Serialize(fs, this);
                fs.Flush();
            }
        }

        public string SaveToString()
        {
            var serializer = new XmlSerializer(GetType());
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, this);
                return sw.ToString();
            }
        }

        public static T Load(string filename)
        {
            if (!File.Exists(filename))
            {
                return default(T);
            }
            var serializer = new XmlSerializer(typeof(T));
            using (var fs = File.OpenRead(filename))
            {
                return (T)serializer.Deserialize(fs);
            }
        }

        public static T LoadFromString(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                return default(T);
            }
            var serializer = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(data))
            {
                return (T)serializer.Deserialize(sr);
            }
        }
    }
}
