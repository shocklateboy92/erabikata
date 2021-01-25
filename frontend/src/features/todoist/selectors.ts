import { RootState } from "app/rootReducer";

export const selectIsTodoistInitialized = (state: RootState) => !!state.todoist.authToken