import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import Login from "./pages/Login.jsx"
import Dashboard from "./pages/Dashboard.jsx"
import Register from "./pages/Register.jsx"
import MonthView from "./pages/MonthView";
import './App.css';
import SettingsMenu from './components/SettingsMenu';


function App() {
    return (
        <Router>
        <SettingsMenu />
            <Link to="/dashboard">
                <img
                    src="/homeButton.png"
                    alt="Home"
                    style={{
                        position: 'fixed',
                        top: '20px',
                        left: '20px',
                        width: '60px',
                        height: '60px',
                        cursor: 'pointer',
                        zIndex: 999
                    }}
                    />
            </Link>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/month/:year/:month" element={<MonthView />} />
            </Routes>
        </Router>
    );
}
export default App;