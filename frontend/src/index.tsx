import { App } from 'App';
import ReactDOM from 'react-dom';
import './App.css';
import './app.scss';
import './index.css';
import * as serviceWorker from './serviceWorkerRegistration';

ReactDOM.render(<App />, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();
