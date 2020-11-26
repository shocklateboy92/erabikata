#!/bin/env python3

import glob
import ass
from ass_tag_parser import parse_ass
import ass_tag_parser
import sys
import re
import json
from multiprocessing import Pool

from pathlib import Path
from ass_tag_parser.errors import ParseError

INCLUDE_STYLES = ['K1Norma',
                  'K2Overl', 'K3Thoug', 'K4Notes', 'K5Narra', 'Default - Top', 'Default - Italic', 'flashback', 'top', 'main', 'italics', 'Anohana-alt']
INCLUDE_STYLE_PATTERNS = ['Default', 'Dialogue', 'Alternate']
EXCLUDE_STYLES = ['Default2', 'Alternate2']
SUBS_ROOT = '../anime-subs/english'
ALL_TAGS_REGEX = re.compile(r'{[^}]*}')

files = sys.argv[1:] or glob.glob(f'{SUBS_ROOT}/**/*.ass', recursive=True)
print(f"Processing {len(files)} files")


def do_parse(file: str):
    with open(file) as f:
        try:
            doc = ass.parse(f)
            results = []

            for event in sorted(doc.events, key=lambda x: x.start):
                if event.TYPE != 'Dialogue':
                    continue

                if event.style not in EXCLUDE_STYLES and event.style in INCLUDE_STYLES or any(style.lower() in event.style.lower() for style in INCLUDE_STYLE_PATTERNS):
                    text = ''
                    try:
                        parsed = parse_ass(event.text)
                        text = '\n'.join([a.text for a in parsed if isinstance(
                            a, ass_tag_parser.AssText)])
                    except ParseError:
                        text = ALL_TAGS_REGEX.sub('', event.text)

                    results.append(
                        {'text': text, 'time': event.start.total_seconds(), 'style': event.style})
                else:
                    results.append(
                        {'time': event.start.total_seconds(), 'style': event.style, 'size': len(event.text)})

            with open(Path(file).with_suffix('.json'), 'w') as out:
                json.dump(results, out, indent=2)
        except:
            print(f'failed to parse {f}')


with Pool(12) as pool:
    pool.map(do_parse, files)
