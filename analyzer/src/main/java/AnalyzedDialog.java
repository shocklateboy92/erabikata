public class AnalyzedDialog extends Dialog {
    protected final Word[][] analyzed;

    public AnalyzedDialog(Dialog parent, Word[][] analyzed) {
        this.analyzed = analyzed;
    }

    public static class Word {
        public String original;
        public String base;
        public String[] partOfSpeech;
        public String conjugationType;
        public String reading;
        public String pronunciation;
    }
}
