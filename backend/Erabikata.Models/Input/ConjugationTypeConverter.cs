using System;
using Newtonsoft.Json;

namespace Erabikata.Models.Input
{
    internal class ConjugationTypeConverter : JsonConverter
    {
        public static readonly ConjugationTypeConverter Singleton = new ConjugationTypeConverter();

        public override bool CanConvert(Type t)
        {
            return t == typeof(ConjugationType) || t == typeof(ConjugationType?);
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "*":
                    return ConjugationType.Empty;
                case "カ変・クル":
                    return ConjugationType.カ変クル;
                case "カ変・来ル":
                    return ConjugationType.カ変来ル;
                case "サ変・−スル":
                    return ConjugationType.ConjugationTypeサ変スル;
                case "サ変・−ズル":
                    return ConjugationType.サ変ズル;
                case "サ変・スル":
                    return ConjugationType.サ変スル;
                case "ラ変":
                    return ConjugationType.ラ変;
                case "一段":
                    return ConjugationType.一段;
                case "一段・クレル":
                    return ConjugationType.一段クレル;
                case "一段・ク��ル":
                    return ConjugationType.一段クル;
                case "一段・得ル":
                    return ConjugationType.一段得ル;
                case "上二・ダ行":
                    return ConjugationType.上二ダ行;
                case "下二・タ行":
                    return ConjugationType.下二タ行;
                case "下二・ダ行":
                    return ConjugationType.下二ダ行;
                case "不変化型":
                    return ConjugationType.不変化型;
                case "五段・カ行イ音便":
                    return ConjugationType.五段カ行イ音便;
                case "五段・カ行促音便":
                    return ConjugationType.五段カ行促音便;
                case "五段・カ行促音便ユク":
                    return ConjugationType.五段カ行促音便ユク;
                case "五段・ガ行":
                    return ConjugationType.五段ガ行;
                case "五段・サ行":
                    return ConjugationType.五段サ行;
                case "五段・タ行":
                    return ConjugationType.五段タ行;
                case "五段・ナ行":
                    return ConjugationType.五段ナ行;
                case "五段・バ行":
                    return ConjugationType.五段バ行;
                case "五段・マ行":
                    return ConjugationType.五段マ行;
                case "五段・ラ行":
                    return ConjugationType.五段ラ行;
                case "五段・ラ行アル":
                    return ConjugationType.五段ラ行アル;
                case "五段・ラ行特殊":
                    return ConjugationType.五段ラ行特殊;
                case "五段・ラ�特殊":
                    return ConjugationType.五段ラ特殊;
                case "五段・ワ行ウ音便":
                    return ConjugationType.五段ワ行ウ音便;
                case "五段・ワ行促音便":
                    return ConjugationType.五段ワ行促音便;
                case "五段・��行":
                    return ConjugationType.五段行;
                case "五段・��行イ音便":
                    return ConjugationType.五段行イ音便;
                case "五段・��行促音便":
                    return ConjugationType.五段行促音便;
                case "五段��ラ行":
                    return ConjugationType.ConjugationType五段ラ行;
                case "四段・ハ行":
                    return ConjugationType.四段ハ行;
                case "四段・バ行":
                    return ConjugationType.四段バ行;
                case "形容詞・アウオ段":
                    return ConjugationType.形容詞アウオ段;
                case "形容詞・イイ":
                    return ConjugationType.形容詞イイ;
                case "形容詞・イ段":
                    return ConjugationType.形容詞イ段;
                case "形容詞・�段":
                    return ConjugationType.形容詞段;
                case "文語・キ":
                    return ConjugationType.文語キ;
                case "文語・ケリ":
                    return ConjugationType.文語ケリ;
                case "文語・ゴトシ":
                    return ConjugationType.文語ゴトシ;
                case "文語・ナリ":
                    return ConjugationType.文語ナリ;
                case "文語・ベシ":
                    return ConjugationType.文語ベシ;
                case "文語・マジ":
                    return ConjugationType.文語マジ;
                case "文語・リ":
                    return ConjugationType.文語リ;
                case "文語・ル":
                    return ConjugationType.文語ル;
                case "特殊・ジャ":
                    return ConjugationType.特殊ジャ;
                case "特殊・タ":
                    return ConjugationType.特殊タ;
                case "特殊・タイ":
                    return ConjugationType.特殊タイ;
                case "特殊・ダ":
                    return ConjugationType.特殊ダ;
                case "特殊・デス":
                    return ConjugationType.特殊デス;
                case "特殊・ナイ":
                    return ConjugationType.特殊ナイ;
                case "特殊・ヌ":
                    return ConjugationType.特殊ヌ;
                case "特殊・マス":
                    return ConjugationType.特殊マス;
                case "特殊・ヤ":
                    return ConjugationType.特殊ヤ;
                case "特殊・�":
                    return ConjugationType.特殊;
            }

            throw new Exception("Cannot unmarshal type ConjugationType");
        }

        public override void WriteJson(
            JsonWriter writer,
            object untypedValue,
            JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (ConjugationType) untypedValue;
            switch (value)
            {
                case ConjugationType.Empty:
                    serializer.Serialize(writer, "*");
                    return;
                case ConjugationType.カ変クル:
                    serializer.Serialize(writer, "カ変・クル");
                    return;
                case ConjugationType.カ変来ル:
                    serializer.Serialize(writer, "カ変・来ル");
                    return;
                case ConjugationType.ConjugationTypeサ変スル:
                    serializer.Serialize(writer, "サ変・−スル");
                    return;
                case ConjugationType.サ変ズル:
                    serializer.Serialize(writer, "サ変・−ズル");
                    return;
                case ConjugationType.サ変スル:
                    serializer.Serialize(writer, "サ変・スル");
                    return;
                case ConjugationType.ラ変:
                    serializer.Serialize(writer, "ラ変");
                    return;
                case ConjugationType.一段:
                    serializer.Serialize(writer, "一段");
                    return;
                case ConjugationType.一段クレル:
                    serializer.Serialize(writer, "一段・クレル");
                    return;
                case ConjugationType.一段クル:
                    serializer.Serialize(writer, "一段・ク��ル");
                    return;
                case ConjugationType.一段得ル:
                    serializer.Serialize(writer, "一段・得ル");
                    return;
                case ConjugationType.上二ダ行:
                    serializer.Serialize(writer, "上二・ダ行");
                    return;
                case ConjugationType.下二タ行:
                    serializer.Serialize(writer, "下二・タ行");
                    return;
                case ConjugationType.下二ダ行:
                    serializer.Serialize(writer, "下二・ダ行");
                    return;
                case ConjugationType.不変化型:
                    serializer.Serialize(writer, "不変化型");
                    return;
                case ConjugationType.五段カ行イ音便:
                    serializer.Serialize(writer, "五段・カ行イ音便");
                    return;
                case ConjugationType.五段カ行促音便:
                    serializer.Serialize(writer, "五段・カ行促音便");
                    return;
                case ConjugationType.五段カ行促音便ユク:
                    serializer.Serialize(writer, "五段・カ行促音便ユク");
                    return;
                case ConjugationType.五段ガ行:
                    serializer.Serialize(writer, "五段・ガ行");
                    return;
                case ConjugationType.五段サ行:
                    serializer.Serialize(writer, "五段・サ行");
                    return;
                case ConjugationType.五段タ行:
                    serializer.Serialize(writer, "五段・タ行");
                    return;
                case ConjugationType.五段ナ行:
                    serializer.Serialize(writer, "五段・ナ行");
                    return;
                case ConjugationType.五段バ行:
                    serializer.Serialize(writer, "五段・バ行");
                    return;
                case ConjugationType.五段マ行:
                    serializer.Serialize(writer, "五段・マ行");
                    return;
                case ConjugationType.五段ラ行:
                    serializer.Serialize(writer, "五段・ラ行");
                    return;
                case ConjugationType.五段ラ行アル:
                    serializer.Serialize(writer, "五段・ラ行アル");
                    return;
                case ConjugationType.五段ラ行特殊:
                    serializer.Serialize(writer, "五段・ラ行特殊");
                    return;
                case ConjugationType.五段ラ特殊:
                    serializer.Serialize(writer, "五段・ラ�特殊");
                    return;
                case ConjugationType.五段ワ行ウ音便:
                    serializer.Serialize(writer, "五段・ワ行ウ音便");
                    return;
                case ConjugationType.五段ワ行促音便:
                    serializer.Serialize(writer, "五段・ワ行促音便");
                    return;
                case ConjugationType.五段行:
                    serializer.Serialize(writer, "五段・��行");
                    return;
                case ConjugationType.五段行イ音便:
                    serializer.Serialize(writer, "五段・��行イ音便");
                    return;
                case ConjugationType.五段行促音便:
                    serializer.Serialize(writer, "五段・��行促音便");
                    return;
                case ConjugationType.ConjugationType五段ラ行:
                    serializer.Serialize(writer, "五段��ラ行");
                    return;
                case ConjugationType.四段ハ行:
                    serializer.Serialize(writer, "四段・ハ行");
                    return;
                case ConjugationType.四段バ行:
                    serializer.Serialize(writer, "四段・バ行");
                    return;
                case ConjugationType.形容詞アウオ段:
                    serializer.Serialize(writer, "形容詞・アウオ段");
                    return;
                case ConjugationType.形容詞イイ:
                    serializer.Serialize(writer, "形容詞・イイ");
                    return;
                case ConjugationType.形容詞イ段:
                    serializer.Serialize(writer, "形容詞・イ段");
                    return;
                case ConjugationType.形容詞段:
                    serializer.Serialize(writer, "形容詞・�段");
                    return;
                case ConjugationType.文語キ:
                    serializer.Serialize(writer, "文語・キ");
                    return;
                case ConjugationType.文語ケリ:
                    serializer.Serialize(writer, "文語・ケリ");
                    return;
                case ConjugationType.文語ゴトシ:
                    serializer.Serialize(writer, "文語・ゴトシ");
                    return;
                case ConjugationType.文語ナリ:
                    serializer.Serialize(writer, "文語・ナリ");
                    return;
                case ConjugationType.文語ベシ:
                    serializer.Serialize(writer, "文語・ベシ");
                    return;
                case ConjugationType.文語マジ:
                    serializer.Serialize(writer, "文語・マジ");
                    return;
                case ConjugationType.文語リ:
                    serializer.Serialize(writer, "文語・リ");
                    return;
                case ConjugationType.文語ル:
                    serializer.Serialize(writer, "文語・ル");
                    return;
                case ConjugationType.特殊ジャ:
                    serializer.Serialize(writer, "特殊・ジャ");
                    return;
                case ConjugationType.特殊タ:
                    serializer.Serialize(writer, "特殊・タ");
                    return;
                case ConjugationType.特殊タイ:
                    serializer.Serialize(writer, "特殊・タイ");
                    return;
                case ConjugationType.特殊ダ:
                    serializer.Serialize(writer, "特殊・ダ");
                    return;
                case ConjugationType.特殊デス:
                    serializer.Serialize(writer, "特殊・デス");
                    return;
                case ConjugationType.特殊ナイ:
                    serializer.Serialize(writer, "特殊・ナイ");
                    return;
                case ConjugationType.特殊ヌ:
                    serializer.Serialize(writer, "特殊・ヌ");
                    return;
                case ConjugationType.特殊マス:
                    serializer.Serialize(writer, "特殊・マス");
                    return;
                case ConjugationType.特殊ヤ:
                    serializer.Serialize(writer, "特殊・ヤ");
                    return;
                case ConjugationType.特殊:
                    serializer.Serialize(writer, "特殊・�");
                    return;
            }

            throw new Exception("Cannot marshal type ConjugationType");
        }
    }
}