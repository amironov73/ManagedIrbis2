﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Doi.cs -- DOI
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using AM;
using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Identifiers
{
    //
    // Идентификатор цифрового объекта (также используется
    // словосочетание цифровой идентификатор объекта, ЦИО,
    // digital object identifier, DOI) — стандарт обозначения
    // представленной в сети информации об объекте (обычно,
    // но не обязательно, об электронном документе или
    // цифровом объекте). Информация, содержащаяся в DOI
    // электронного документа, содержит указатель его
    // местонахождения (например, URL), его имя (название),
    // прочие идентификаторы объекта (например, ISBN для
    // электронного образа книги) и ассоциированный с объектом
    // набор описывающих его данных (метаданных)
    // в структурированном и расширяемом виде.
    //
    // DOI имеет некоторые общие черты со стандартом PURL:
    // наличие указателя местонахождения объекта
    // и его имени (названия).
    // DOI принят в англоязычной научной среде для обмена
    // данными между учёными. По сути, DOI — это путь к документу
    // в общем информационно-виртуальном пространстве
    // (как правило, в Интернете), для получения необходимой информации.
    //
    // Структура
    // Идентификатор цифрового объекта представляет собой
    // уникальную строку букв и цифр, состоящую из двух частей:
    // префикс и суффикс. Например,
    // 10.1000/182,
    // где
    // 10.1000 — префикс, или идентификатор издателя, составленный
    // из признака идентификатора (10) и строки, указывающей
    // на издателя (1000);
    // 182 — суффикс, идентификатор объекта, указывающий
    // на конкретный объект.
    // Префиксы издателей распределяются регистрационным агентством
    // (DOI Registration Agency) CrossRef. Суффикс формируется
    // издателем, и должен быть уникальным у данного издателя.
    // Идентификатор цифрового объекта может объединить существующие
    // идентификаторы, такие как ISBN,
    // International Standard Serial Number или SICI.
    //
    // Идентификатор цифрового объекта регистронезависим.
    //
    // Примеры
    // DOI 10.1007/b136753
    // Это цифровая копия книги 2006 года «Magnetic Functions
    // Beyond Spin-Hamiltonian» (ISBN 3-540-26079-X),
    // изданной в Берлине под редакцией профессора
    // D. Michael P. Mingos, входящей под №117 в серию-журнал
    // «Structure & Bonding» (ISSN 0081-5993 редакции
    // D. Michael P. Mingos) издательства «Springer-Verlag Берлин
    // Хайдельберг» (в составе Springer Science+Business Media).
    // Книга так же имеет Контрольный номер библиотеки конгресса
    // США (en:Library of Congress Control Number (LCCN)) 2005926235.
    //
    // DOI 10.1007/978-3-540-46129-6
    // Это цифровая копия книги 2007 года «Organometalliс
    // Chemistry & Catalysis» (ISBN 978-3-540-46129-6)
    // Didier Astruc (члена IUF), изданной в «Springer-Verlag Berlin
    // Heidelberg» на английском языке. LCCN 2007924912.
    // В оригинале, содержимое этой книги было опубликовано
    // в 2000 году на французском языке в книге
    // «Chimie Organométallique» (ISBN 2-86883-493-0)
    // издательства «EDP Sciences Гренобль».
    //
    // Также существуют цифровые «DOI-копии» документов,
    // которые нигде ранее не публиковались и были изначально
    // в цифровом виде.

    // Ссылки
    // http://www.doi.org/
    // http://crossref.org/
    // http://pdf.livejournal.com/438166.html

    /// <summary>
    /// DOI
    /// </summary>
    [PublicAPI]
    [XmlRoot("doi")]
    [DebuggerDisplay("{" + nameof(Prefix) + "} {" + nameof(Suffix) + "}")]
    public sealed class Doi
        : IHandmadeSerializable
    {
        #region Properties

        /// <summary>
        /// Префикс.
        /// </summary>
        [CanBeNull]
        [XmlAttribute("prefix")]
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// Суффикс.
        /// </summary>
        [CanBeNull]
        [XmlAttribute("suffix")]
        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        /// <summary>
        /// Arbitrary user data.
        /// </summary>
        [CanBeNull]
        [XmlIgnore]
        [JsonIgnore]
        [Browsable(false)]
        public object UserData { get; set; }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            Prefix = reader.ReadNullableString();
            Suffix = reader.ReadNullableString();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer
                .WriteNullable(Prefix)
                .WriteNullable(Suffix);
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return $"{Prefix}/{Suffix}";
        }

        #endregion
    }
}
