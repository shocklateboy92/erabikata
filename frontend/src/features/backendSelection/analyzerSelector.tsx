import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { Analyzer } from 'backend.generated';
import { FullWidthText } from 'components/fullWidth';
import React, { FC } from 'react';
import { selectAnalyzer } from '.';
import { analyzerChangeRequest } from './slice';

const analyzerTexts: {
    [key in Analyzer]: string;
} = {
    Kuromoji: 'Kuromoji',
    SudachiA: 'Sudachi, Mode A',
    SudachiB: 'Sudachi, Mode B',
    SudachiC: 'Sudachi, Mode C'
} as const;

export const AnalyzerSelector: FC = () => {
    const dispatch = useAppDispatch();
    const currentAnaylzer = useTypedSelector(selectAnalyzer);
    return (
        <FullWidthText>
            <h2>Use Analyzer</h2>
            <form>
                {Object.keys(analyzerTexts).map((strKey) => {
                    // TODO: Figure out how to not widen this type
                    const an = strKey as Analyzer;
                    return (
                        <div key={an}>
                            <input
                                name="analyzer"
                                type="radio"
                                id={an}
                                checked={currentAnaylzer === an}
                                onChange={(e) =>
                                    e.target.checked &&
                                    dispatch(analyzerChangeRequest(an))
                                }
                            />
                            <label htmlFor={an}>{analyzerTexts[an]}</label>
                        </div>
                    );
                })}
            </form>
        </FullWidthText>
    );
};
