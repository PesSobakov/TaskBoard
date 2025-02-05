import './App.css';
import { Outlet } from 'react-router';
import Navigation from './app/Navigation';

function App()
{
    return (
        <div>
            <Navigation />
            <div>App</div>
            <div><Outlet /></div>
        </div>
    );
}

export default App;