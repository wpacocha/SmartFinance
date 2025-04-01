import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Login() {
    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const res = await fetch("http://localhost:5201/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ username, password })
            });

            if (!res.ok) {
                throw new Error("Login failed.");
            }

            const data = await res.json();
            localStorage.setItem("token", data.token);
            navigate("/dashboard");
        } catch (err) {
            alert("Invalid login.");
        }
    };
    return (
        <form onSubmit={handleSubmit}>
            <input
                placeholder="Username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
            />
            <input
                placeholder="Password"
                value={password}
                type="password"
                onChange={(e) => setPassword(e.target.value)}
            />
            <button type="submit">Login</button>
            <p>No account? <span onClick={() => navigate('/register')} style={{ cursor: 'pointer' }}>Register!</span></p>
        </form>
    );
}