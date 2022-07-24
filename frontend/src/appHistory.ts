import { createBrowserHistory, createMemoryHistory } from 'history';
import { isTest } from 'testSelectors';

export default isTest ? createMemoryHistory() : createBrowserHistory();
