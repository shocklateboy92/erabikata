import {
    AsyncThunk,
    createAsyncThunk,
    createEntityAdapter,
    createSlice
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import axios from 'axios';
import * as jisho from 'unofficial-jisho-api';

export interface IJishoWord {
    isCommon: boolean;
    slug: string;
    japanese: { word: string; reading: string }[];
    english: {
        tags: string[];
        senses: string[];
    }[];
}

export interface IWordDefinition {
    baseForm: string;
    exact: IJishoWord[];
    related: IJishoWord[];
}

const adapter = createEntityAdapter<IWordDefinition>({
    selectId: (word) => word.baseForm
});
const thunk: AsyncThunk<
    jisho.JishoAPIResult,
    string,
    { state: RootState }
> = createAsyncThunk(
    'wordDefinitions',
    async (baseForm) => {
        const repsonse = await axios.get<jisho.JishoAPIResult>(
            `https://erabikata.apps.lasath.org/api/jisho/api/v1/search/words?keyword=${baseForm}`
        );
        return repsonse.data;
    },
    {
        condition: (baseForm, { getState }) => {
            return !selectDefinitionById(getState(), baseForm);
        }
    }
);

/**
 * Groups a list of word senses by tags and maps them to our format
 * @param senses Jisho word senses from API response
 */
const mapSenses = (senses: jisho.JishoWordSense[]) =>
    // We reduce right, because destructuring can only separate the
    // first element. So we map it backwards, taking the "first" of
    // the previously mapped elements for comparison.
    senses.reduceRight(([prev, ...rest], next) => {
        // These are always used together, so we may as well
        // combine them here and save some work in the UI
        const newTags = next.parts_of_speech.concat(next.tags);

        // Base case
        if (!prev) {
            return [
                {
                    tags: newTags,
                    senses: [next.english_definitions.join('; ')]
                }
            ];
        }

        // If tags of the new one is the same as the last mapped one,
        // combine new one into last mapped object.
        if (
            prev.tags.length === newTags.length &&
            prev.tags.every((tag, index) => tag === newTags[index])
        ) {
            return [
                {
                    tags: prev.tags,
                    senses: [
                        next.english_definitions.join('; '),
                        ...prev.senses
                    ]
                },
                ...rest
            ];
        }

        // Otherwise just append a newly mapped object.
        return [
            {
                tags: next.parts_of_speech.concat(next.tags),
                senses: [next.english_definitions.join('; ')]
            },
            prev,
            ...rest
        ];
    }, [] as IJishoWord['english']);

const slice = createSlice({
    name: 'wordDefinitions',
    initialState: adapter.getInitialState(),
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(
            thunk.fulfilled,
            (state, { payload: { data, meta }, meta: { arg: baseForm } }) => {
                if (meta.status !== 200) {
                    return;
                }

                adapter.upsertOne(
                    state,
                    data.reduce(
                        ({ baseForm, exact, related }, next) => {
                            if (
                                next.japanese.find(
                                    (j) =>
                                        j.word === baseForm ||
                                        j.reading === baseForm
                                )
                            ) {
                                return {
                                    baseForm,
                                    exact: [
                                        ...exact,
                                        {
                                            isCommon: next.is_common,
                                            slug: next.slug,
                                            japanese: next.japanese,
                                            english: mapSenses(next.senses)
                                        }
                                    ],
                                    related
                                };
                            } else {
                                return {
                                    baseForm,
                                    exact,
                                    related: [
                                        ...related,
                                        {
                                            isCommon: next.is_common,
                                            slug: next.slug,
                                            japanese: next.japanese,
                                            english: mapSenses(next.senses)
                                        }
                                    ]
                                };
                            }
                        },
                        {
                            baseForm,
                            exact: [],
                            related: []
                        } as IWordDefinition
                    )
                );
            }
        )
});

export const { selectById: selectDefinitionById } = adapter.getSelectors<
    RootState
>((state) => state.wordDefinitions);

export const fetchDefinitionsIfNeeded = thunk;

export const wordDefinitionReducer = slice.reducer;
