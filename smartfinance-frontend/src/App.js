import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./pages/Login.jsx"
import Dashboard from "./pages/Dashboard.jsx"
import Register from "./pages/Register.jsx"
import MonthView from "./pages/MonthView";
function App() {
  return (
      <Router>
        <Routes>
          <Route path="/" element={<Login/>}/>
            <Route path="/register" element={<Register/>}/>
            <Route path="/dashboard" element={<Dashboard/>}/>
            <Route path="/month/:year/:month" element={<MonthView />} />
        </Routes>
      </Router>
  );
}
export default App;