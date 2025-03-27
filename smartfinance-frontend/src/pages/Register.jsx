import {useState} from "react";
import {useNavigate} from "react-router-dom";

export default function Register() {
    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    
    const isPasswordValid = (password) => {
        const hasUpper = /[A-Z]/.test(password);
        const hasLower = /[a-z]/.test(password);
        const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);
        const hasLength = password.length >= 6;
        
        return hasUpper && hasLower && hasSpecial && hasLength;
    };
    
    const handleSubmit = async(e) => {
        e.preventDefault();
        
        if(!isPasswordValid(password)) {
            alert('Password must be at least 6 characters long, contain 1 uppercase letter, 1 lowercase letter and 1 special character.');
            return;
        }
        
        if(password !== confirmPassword) {
            alert('Passwords do not match');
            return;
        }
        
        try{
            const res = await fetch(`${process.env.REACT_APP_API_URL}/Auth/register`,{
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({username, password})
            });
            
            if(!res.ok) throw new Error("Registration failed.");
            
            alert('Registration successfull!');
            navigate("/");
        } catch (err) {
            alert("Registration error.");
        }
    };
    return (
        <div id="register-page">
            <h2>Register</h2>
            <form onSubmit={handleSubmit}>
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
                <input
                    placeholder="Confirm Password"
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                    />
                <button type="submit">Register</button>
                <p>Password requirements:</p>
                <ul>
                    <li>At least 6 characters</li>
                    <li>1 uppercase letter [A-Z]</li>
                    <li>1 lowercase letter [a-z]</li>
                    <li>1 special character (!@#$%^&amp;*(),.?":{}|&lt;&gt; etc.)</li>
                </ul>
            </form>
            <p>Already have an account? <span onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>Log in!</span></p>
        </div>
    );
}