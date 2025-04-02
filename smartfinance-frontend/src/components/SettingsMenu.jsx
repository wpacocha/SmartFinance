import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { FaCog } from "react-icons/fa";
import "./SettingsMenu.css";

export default function SettingsMenu() {
    const [showSettings, setShowSettings] = useState(false);
    const [currency, setCurrency] = useState(localStorage.getItem("currency") || "PLN");
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem("token");
        fetch("http://localhost:5201/api/user/me", {
            headers: { Authorization: `Bearer ${token}` }
        })
            .then((res) => res.json())
            .then((data) => setCurrency(data.preferredCurrency || "PLN"))
            .catch((err) => console.error("Failed to fetch user", err));
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/");
    };

    const handleCurrencyChange = async (e) => {
        const selected = e.target.value;
        try {
            const res = await fetch("http://localhost:5201/api/user/currency", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                },
                body: JSON.stringify({ Currency: selected })
            });

            if (!res.ok) throw new Error("Currency update failed!");

            window.location.reload();
        } catch (err) {
            console.error("Error changing currency: ", err);
            alert("Error while updating currency.");
        }
    };

    return (
        <div style={{
            position: "fixed",
            top: "20px",
            right: "20px",
            zIndex: 1000
        }}>
            <FaCog
                size={30}
                style={{ cursor: "pointer" }}
                onClick={() => setShowSettings(prev => !prev)}
            />
            {showSettings && (
                <div className="settings-box">
                    <button className="logout-btn" onClick={handleLogout}>Logout</button>
                    <div style={{ marginTop: "10px" }}>
                        <label>
                            Currency: 
                            <select value={currency} onChange={handleCurrencyChange}>
                                <option value="PLN">PLN</option>
                                <option value="USD">USD</option>
                                <option value="EUR">EUR</option>
                                <option value="GBP">GBP</option>
                                <option value="CHF">CHF</option>
                                <option value="NOK">NOK</option>
                                <option value="SEK">SEK</option>
                                <option value="CZK">CZK</option>
                                <option value="CAD">CAD</option>
                                <option value="AUD">AUD</option>
                            </select>
                        </label>
                    </div>
                </div>
            )}
        </div>
    );
}
