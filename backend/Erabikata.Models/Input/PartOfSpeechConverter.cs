using System;
using Newtonsoft.Json;

namespace Erabikata.Models.Input
{
    internal class PartOfSpeechConverter : JsonConverter
    {
        public static readonly PartOfSpeechConverter Singleton = new PartOfSpeechConverter();

        public override bool CanConvert(Type t)
        {
            return t == typeof(PartOfSpeech) || t == typeof(PartOfSpeech?);
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
                    return PartOfSpeech.Empty;
                case "その他":
                    return PartOfSpeech.その他;
                case "アルファベット":
                    return PartOfSpeech.アルファベット;
                case "サ変接続":
                    return PartOfSpeech.サ変接続;
                case "サ変��続":
                    return PartOfSpeech.サ変続;
                case "サ��接続":
                    return PartOfSpeech.サ接続;
                case "ナイ形容詞語幹":
                    return PartOfSpeech.ナイ形容詞語幹;
                case "フィラー":
                    return PartOfSpeech.フィラー;
                case "一般":
                    return PartOfSpeech.一般;
                case "一��":
                    return PartOfSpeech.一;
                case "並立助詞":
                    return PartOfSpeech.並立助詞;
                case "人名":
                    return PartOfSpeech.人名;
                case "代名詞":
                    return PartOfSpeech.代名詞;
                case "代名��":
                    return PartOfSpeech.代名;
                case "係助詞":
                    return PartOfSpeech.係助詞;
                case "副助詞":
                    return PartOfSpeech.副助詞;
                case "副助詞／並立助詞／終助詞":
                    return PartOfSpeech.副助詞並立助詞終助詞;
                case "副助詞／並立助詞��終助詞":
                    return PartOfSpeech.PartOfSpeech副助詞並立助詞終助詞;
                case "副詞":
                    return PartOfSpeech.副詞;
                case "副詞化":
                    return PartOfSpeech.副詞化;
                case "副詞可能":
                    return PartOfSpeech.副詞可能;
                case "副詞可�":
                    return PartOfSpeech.副詞可;
                case "助動詞":
                    return PartOfSpeech.助動詞;
                case "助動詞語幹":
                    return PartOfSpeech.助動詞語幹;
                case "助動�":
                    return PartOfSpeech.助動;
                case "助数詞":
                    return PartOfSpeech.助数詞;
                case "助詞":
                    return PartOfSpeech.助詞;
                case "助詞類接続":
                    return PartOfSpeech.助詞類接続;
                case "助�":
                    return PartOfSpeech.助;
                case "動詞":
                    return PartOfSpeech.動詞;
                case "動詞接続":
                    return PartOfSpeech.動詞接続;
                case "動詞非自立的":
                    return PartOfSpeech.動詞非自立的;
                case "動�":
                    return PartOfSpeech.動;
                case "句点":
                    return PartOfSpeech.句点;
                case "名":
                    return PartOfSpeech.名;
                case "名詞":
                    return PartOfSpeech.名詞;
                case "名詞接続":
                    return PartOfSpeech.名詞接続;
                case "名�":
                    return PartOfSpeech.Purple名;
                case "名��":
                    return PartOfSpeech.PartOfSpeech名;
                case "固有名詞":
                    return PartOfSpeech.固有名詞;
                case "国":
                    return PartOfSpeech.国;
                case "地域":
                    return PartOfSpeech.地域;
                case "姓":
                    return PartOfSpeech.姓;
                case "引用":
                    return PartOfSpeech.引用;
                case "引用文字列":
                    return PartOfSpeech.引用文字列;
                case "形容動詞語幹":
                    return PartOfSpeech.形容動詞語幹;
                case "形容詞":
                    return PartOfSpeech.形容詞;
                case "形容詞接続":
                    return PartOfSpeech.形容詞接続;
                case "感動詞":
                    return PartOfSpeech.感動詞;
                case "感��詞":
                    return PartOfSpeech.感詞;
                case "括弧閉":
                    return PartOfSpeech.括弧閉;
                case "括弧開":
                    return PartOfSpeech.括弧開;
                case "括弧�":
                    return PartOfSpeech.括弧;
                case "接尾":
                    return PartOfSpeech.接尾;
                case "接続助詞":
                    return PartOfSpeech.接続助詞;
                case "接続助��":
                    return PartOfSpeech.接続助;
                case "接続詞":
                    return PartOfSpeech.接続詞;
                case "接続詞的":
                    return PartOfSpeech.接続詞的;
                case "接続�詞":
                    return PartOfSpeech.PartOfSpeech接続詞;
                case "接頭詞":
                    return PartOfSpeech.接頭詞;
                case "数":
                    return PartOfSpeech.数;
                case "数接続":
                    return PartOfSpeech.数接続;
                case "格助詞":
                    return PartOfSpeech.格助詞;
                case "特殊":
                    return PartOfSpeech.特殊;
                case "空白":
                    return PartOfSpeech.空白;
                case "終助詞":
                    return PartOfSpeech.終助詞;
                case "組織":
                    return PartOfSpeech.組織;
                case "縮約":
                    return PartOfSpeech.縮約;
                case "自立":
                    return PartOfSpeech.自立;
                case "自�":
                    return PartOfSpeech.自;
                case "記号":
                    return PartOfSpeech.記号;
                case "記�":
                    return PartOfSpeech.記;
                case "記��":
                    return PartOfSpeech.PartOfSpeech記;
                case "読点":
                    return PartOfSpeech.読点;
                case "連体化":
                    return PartOfSpeech.連体化;
                case "連体詞":
                    return PartOfSpeech.連体詞;
                case "連語":
                    return PartOfSpeech.連語;
                case "間投":
                    return PartOfSpeech.間投;
                case "非自立":
                    return PartOfSpeech.非自立;
                case "非�立":
                    return PartOfSpeech.非立;
                case "�助詞":
                    return PartOfSpeech.Purple助詞;
                case "�助詞／並立助詞／終助詞":
                    return PartOfSpeech.助詞並立助詞終助詞;
                case "�動詞":
                    return PartOfSpeech.PartOfSpeech動詞;
                case "�名詞":
                    return PartOfSpeech.PartOfSpeech名詞;
                case "�自立":
                    return PartOfSpeech.PartOfSpeech自立;
                case "�般":
                    return PartOfSpeech.PartOfSpeech般;
                case "�詞":
                    return PartOfSpeech.詞;
                case "��助詞":
                    return PartOfSpeech.PartOfSpeech助詞;
                case "��変接続":
                    return PartOfSpeech.変接続;
                case "��立":
                    return PartOfSpeech.立;
                case "��続詞":
                    return PartOfSpeech.続詞;
                case "��般":
                    return PartOfSpeech.般;
                case "��詞":
                    return PartOfSpeech.PartOfSpeech詞;
            }

            throw new Exception("Cannot unmarshal type PartOfSpeech");
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

            var value = (PartOfSpeech) untypedValue;
            switch (value)
            {
                case PartOfSpeech.Empty:
                    serializer.Serialize(writer, "*");
                    return;
                case PartOfSpeech.その他:
                    serializer.Serialize(writer, "その他");
                    return;
                case PartOfSpeech.アルファベット:
                    serializer.Serialize(writer, "アルファベット");
                    return;
                case PartOfSpeech.サ変接続:
                    serializer.Serialize(writer, "サ変接続");
                    return;
                case PartOfSpeech.サ変続:
                    serializer.Serialize(writer, "サ変��続");
                    return;
                case PartOfSpeech.サ接続:
                    serializer.Serialize(writer, "サ��接続");
                    return;
                case PartOfSpeech.ナイ形容詞語幹:
                    serializer.Serialize(writer, "ナイ形容詞語幹");
                    return;
                case PartOfSpeech.フィラー:
                    serializer.Serialize(writer, "フィラー");
                    return;
                case PartOfSpeech.一般:
                    serializer.Serialize(writer, "一般");
                    return;
                case PartOfSpeech.一:
                    serializer.Serialize(writer, "一��");
                    return;
                case PartOfSpeech.並立助詞:
                    serializer.Serialize(writer, "並立助詞");
                    return;
                case PartOfSpeech.人名:
                    serializer.Serialize(writer, "人名");
                    return;
                case PartOfSpeech.代名詞:
                    serializer.Serialize(writer, "代名詞");
                    return;
                case PartOfSpeech.代名:
                    serializer.Serialize(writer, "代名��");
                    return;
                case PartOfSpeech.係助詞:
                    serializer.Serialize(writer, "係助詞");
                    return;
                case PartOfSpeech.副助詞:
                    serializer.Serialize(writer, "副助詞");
                    return;
                case PartOfSpeech.副助詞並立助詞終助詞:
                    serializer.Serialize(writer, "副助詞／並立助詞／終助詞");
                    return;
                case PartOfSpeech.PartOfSpeech副助詞並立助詞終助詞:
                    serializer.Serialize(writer, "副助詞／並立助詞��終助詞");
                    return;
                case PartOfSpeech.副詞:
                    serializer.Serialize(writer, "副詞");
                    return;
                case PartOfSpeech.副詞化:
                    serializer.Serialize(writer, "副詞化");
                    return;
                case PartOfSpeech.副詞可能:
                    serializer.Serialize(writer, "副詞可能");
                    return;
                case PartOfSpeech.副詞可:
                    serializer.Serialize(writer, "副詞可�");
                    return;
                case PartOfSpeech.助動詞:
                    serializer.Serialize(writer, "助動詞");
                    return;
                case PartOfSpeech.助動詞語幹:
                    serializer.Serialize(writer, "助動詞語幹");
                    return;
                case PartOfSpeech.助動:
                    serializer.Serialize(writer, "助動�");
                    return;
                case PartOfSpeech.助数詞:
                    serializer.Serialize(writer, "助数詞");
                    return;
                case PartOfSpeech.助詞:
                    serializer.Serialize(writer, "助詞");
                    return;
                case PartOfSpeech.助詞類接続:
                    serializer.Serialize(writer, "助詞類接続");
                    return;
                case PartOfSpeech.助:
                    serializer.Serialize(writer, "助�");
                    return;
                case PartOfSpeech.動詞:
                    serializer.Serialize(writer, "動詞");
                    return;
                case PartOfSpeech.動詞接続:
                    serializer.Serialize(writer, "動詞接続");
                    return;
                case PartOfSpeech.動詞非自立的:
                    serializer.Serialize(writer, "動詞非自立的");
                    return;
                case PartOfSpeech.動:
                    serializer.Serialize(writer, "動�");
                    return;
                case PartOfSpeech.句点:
                    serializer.Serialize(writer, "句点");
                    return;
                case PartOfSpeech.名:
                    serializer.Serialize(writer, "名");
                    return;
                case PartOfSpeech.名詞:
                    serializer.Serialize(writer, "名詞");
                    return;
                case PartOfSpeech.名詞接続:
                    serializer.Serialize(writer, "名詞接続");
                    return;
                case PartOfSpeech.Purple名:
                    serializer.Serialize(writer, "名�");
                    return;
                case PartOfSpeech.PartOfSpeech名:
                    serializer.Serialize(writer, "名��");
                    return;
                case PartOfSpeech.固有名詞:
                    serializer.Serialize(writer, "固有名詞");
                    return;
                case PartOfSpeech.国:
                    serializer.Serialize(writer, "国");
                    return;
                case PartOfSpeech.地域:
                    serializer.Serialize(writer, "地域");
                    return;
                case PartOfSpeech.姓:
                    serializer.Serialize(writer, "姓");
                    return;
                case PartOfSpeech.引用:
                    serializer.Serialize(writer, "引用");
                    return;
                case PartOfSpeech.引用文字列:
                    serializer.Serialize(writer, "引用文字列");
                    return;
                case PartOfSpeech.形容動詞語幹:
                    serializer.Serialize(writer, "形容動詞語幹");
                    return;
                case PartOfSpeech.形容詞:
                    serializer.Serialize(writer, "形容詞");
                    return;
                case PartOfSpeech.形容詞接続:
                    serializer.Serialize(writer, "形容詞接続");
                    return;
                case PartOfSpeech.感動詞:
                    serializer.Serialize(writer, "感動詞");
                    return;
                case PartOfSpeech.感詞:
                    serializer.Serialize(writer, "感��詞");
                    return;
                case PartOfSpeech.括弧閉:
                    serializer.Serialize(writer, "括弧閉");
                    return;
                case PartOfSpeech.括弧開:
                    serializer.Serialize(writer, "括弧開");
                    return;
                case PartOfSpeech.括弧:
                    serializer.Serialize(writer, "括弧�");
                    return;
                case PartOfSpeech.接尾:
                    serializer.Serialize(writer, "接尾");
                    return;
                case PartOfSpeech.接続助詞:
                    serializer.Serialize(writer, "接続助詞");
                    return;
                case PartOfSpeech.接続助:
                    serializer.Serialize(writer, "接続助��");
                    return;
                case PartOfSpeech.接続詞:
                    serializer.Serialize(writer, "接続詞");
                    return;
                case PartOfSpeech.接続詞的:
                    serializer.Serialize(writer, "接続詞的");
                    return;
                case PartOfSpeech.PartOfSpeech接続詞:
                    serializer.Serialize(writer, "接続�詞");
                    return;
                case PartOfSpeech.接頭詞:
                    serializer.Serialize(writer, "接頭詞");
                    return;
                case PartOfSpeech.数:
                    serializer.Serialize(writer, "数");
                    return;
                case PartOfSpeech.数接続:
                    serializer.Serialize(writer, "数接続");
                    return;
                case PartOfSpeech.格助詞:
                    serializer.Serialize(writer, "格助詞");
                    return;
                case PartOfSpeech.特殊:
                    serializer.Serialize(writer, "特殊");
                    return;
                case PartOfSpeech.空白:
                    serializer.Serialize(writer, "空白");
                    return;
                case PartOfSpeech.終助詞:
                    serializer.Serialize(writer, "終助詞");
                    return;
                case PartOfSpeech.組織:
                    serializer.Serialize(writer, "組織");
                    return;
                case PartOfSpeech.縮約:
                    serializer.Serialize(writer, "縮約");
                    return;
                case PartOfSpeech.自立:
                    serializer.Serialize(writer, "自立");
                    return;
                case PartOfSpeech.自:
                    serializer.Serialize(writer, "自�");
                    return;
                case PartOfSpeech.記号:
                    serializer.Serialize(writer, "記号");
                    return;
                case PartOfSpeech.記:
                    serializer.Serialize(writer, "記�");
                    return;
                case PartOfSpeech.PartOfSpeech記:
                    serializer.Serialize(writer, "記��");
                    return;
                case PartOfSpeech.読点:
                    serializer.Serialize(writer, "読点");
                    return;
                case PartOfSpeech.連体化:
                    serializer.Serialize(writer, "連体化");
                    return;
                case PartOfSpeech.連体詞:
                    serializer.Serialize(writer, "連体詞");
                    return;
                case PartOfSpeech.連語:
                    serializer.Serialize(writer, "連語");
                    return;
                case PartOfSpeech.間投:
                    serializer.Serialize(writer, "間投");
                    return;
                case PartOfSpeech.非自立:
                    serializer.Serialize(writer, "非自立");
                    return;
                case PartOfSpeech.非立:
                    serializer.Serialize(writer, "非�立");
                    return;
                case PartOfSpeech.Purple助詞:
                    serializer.Serialize(writer, "�助詞");
                    return;
                case PartOfSpeech.助詞並立助詞終助詞:
                    serializer.Serialize(writer, "�助詞／並立助詞／終助詞");
                    return;
                case PartOfSpeech.PartOfSpeech動詞:
                    serializer.Serialize(writer, "�動詞");
                    return;
                case PartOfSpeech.PartOfSpeech名詞:
                    serializer.Serialize(writer, "�名詞");
                    return;
                case PartOfSpeech.PartOfSpeech自立:
                    serializer.Serialize(writer, "�自立");
                    return;
                case PartOfSpeech.PartOfSpeech般:
                    serializer.Serialize(writer, "�般");
                    return;
                case PartOfSpeech.詞:
                    serializer.Serialize(writer, "�詞");
                    return;
                case PartOfSpeech.PartOfSpeech助詞:
                    serializer.Serialize(writer, "��助詞");
                    return;
                case PartOfSpeech.変接続:
                    serializer.Serialize(writer, "��変接続");
                    return;
                case PartOfSpeech.立:
                    serializer.Serialize(writer, "��立");
                    return;
                case PartOfSpeech.続詞:
                    serializer.Serialize(writer, "��続詞");
                    return;
                case PartOfSpeech.般:
                    serializer.Serialize(writer, "��般");
                    return;
                case PartOfSpeech.PartOfSpeech詞:
                    serializer.Serialize(writer, "��詞");
                    return;
            }

            throw new Exception("Cannot marshal type PartOfSpeech");
        }
    }
}