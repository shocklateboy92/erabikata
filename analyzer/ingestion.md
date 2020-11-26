# Current steps for adding new Jap subs

## From embedded video files

### Extract subs into SRT

```
for file in  /mnt/net/media/tv/Terrace.House.Tokyo.2019.2020.S01.1080p.NF.WEB-DL.DDP2.0.x264-AJP69/*
do
ffmpeg -i $file -map 0:3 -f srt "$(basename $file).srt"
done
```

Where `0:3` is the subtitle track number from `ffmpeg -i file_path.mkv`.

### Rename to match convention
```
for file in *                                     :(
do
mv "$file" "$(echo $file | sed -E 's/Terrace.House.Tokyo.2019.2020.S01.([0-9]{2}).*/Terrace House Episode \1.srt/g')"
done
```

### Run srt-parser

This converts them to JSON format that analyzer can understand. First do a `dotnet build` in the `srt-parser` directory, then:

```
$IKNOW_DIR/srt-parser/bin/Debug/netcoreapp3.1/srt-parser
```

### Run the Analyzer

This uses kuromoji to separate out the words

First build the analyzer:

```
cd analyzer;
mvn package
```

Command to run it, anywhere in the anime dir. Run closer to subs to scope workload.

```
java -jar $IKNOW_DIR/analyzer/target/analyzer-1.0-SNAPSHOT-jar-with-dependencies.jar
```

### Add video pattern mapping

Copy existing pattern from somewhere else and modify to fit.
Use `/metadata/episodes` page to verify and debug.

