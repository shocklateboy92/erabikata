import { RootState } from './rootReducer';
import { shallowEqual, useSelector } from 'react-redux';

export const useAppSelector = <T>(selector: (state: RootState) => T) =>
    useSelector(selector, shallowEqual);
