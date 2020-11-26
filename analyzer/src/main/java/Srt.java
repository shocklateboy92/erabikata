import com.google.gson.Gson;
import com.jonathanedgecombe.srt.InvalidTimestampFormatException;
import com.jonathanedgecombe.srt.Subtitle;
import com.jonathanedgecombe.srt.SubtitleFile;

import java.io.FileWriter;
import java.io.IOException;
import java.io.Writer;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class Srt {
    public static void main(String[] args) throws IOException {
        var gson = new Gson();

        Files.walk(Paths.get(".")).filter(p -> p.toString().endsWith(".srt")).forEach(p -> {
            try {
                System.out.println("Processing " + p);
                List<Dialog> lines = new SubtitleFile(p.toFile()).getSubtitles().stream().map(line -> new Dialog() {{
                    startTime = ((double) line.getStartTime().getMilliseconds()) / 1000;
                    tokenized = new String[]{String.join("\n", line.getLines())};
                }}).collect(Collectors.toList());

                var analyzed = Main.analyzeLines(lines).toArray(AnalyzedDialog[]::new);
                Arrays.stream(analyzed).forEach(ad -> ad.tokenized = Arrays.stream(ad.analyzed).map(d -> d.original).toArray(String[]::new));
                var outPath = p.resolveSibling(p.getFileName().toString().replaceAll("\\.srt$", ".json"));

                try (Writer writer = new FileWriter(outPath.toFile())) {
                    gson.toJson(analyzed, writer);
                }
            } catch (Exception e) {
                System.err.println("Error processing file: " + p);
                e.printStackTrace();
            }
        });
    }
}
