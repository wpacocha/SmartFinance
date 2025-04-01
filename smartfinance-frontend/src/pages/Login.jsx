import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Login.css";

export default function Login() {
    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const res = await fetch("http://localhost:5201/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password }),
            });

            if (!res.ok) throw new Error("Login failed.");

            const data = await res.json();
            localStorage.setItem("token", data.token);
            navigate("/dashboard");
        } catch (err) {
            alert("Invalid login.");
        }
    };

    return (
        <div className="login-container">
            <form onSubmit={handleSubmit} className="login-box">
                <h2>Log in to SmartFinance</h2>
                <input
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                <input
                    placeholder="Password"
                    value={password}
                    type="password"
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <button type="submit">Login</button>
                <p>
                    No account?{" "}
                    <span onClick={() => navigate("/register")}>Register!</span>
                </p>
            </form>
        </div>
    );
}
