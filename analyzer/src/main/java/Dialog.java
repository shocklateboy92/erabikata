public class Dialog {
    protected double startTime;
    protected String[] tokenized;

    public String getText() {
        return String.join("", this.tokenized);
    }
}
