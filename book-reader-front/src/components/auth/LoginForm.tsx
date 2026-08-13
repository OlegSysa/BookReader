import { useState } from "react";
import { Login } from "../../api/apiClientMain";
import { useNavigate } from "react-router-dom";
import "./AuthStyles.css";

interface LoginFormProps {
    onClose: () => void;
}

export default function LoginForm({ onClose }: LoginFormProps) {


    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const handleLoginButtonClick = async () => {
        const result = await Login(login, password);
        if (result.success) {
            onClose();
            navigate("/dashboard");
        }
        else {
            setError(result.errorMessage ?? "Failed to login");
        }
    };
    return (
        <div className="auth-form">

            <div className="auth-form-header">
                <button
                    type="button"
                    className="auth-form-close"
                    onClick={onClose}
                >
                    ×
                </button>
                <h2>Login</h2>
            </div>


            <input
                type="text"
                placeholder="Login"
                value={login}
                onChange={(e) => setLogin(e.target.value)}
            />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />
            {error && <div className="auth-form-error">{error}</div>}
            <button onClick={handleLoginButtonClick} type="button">
                Login
            </button>
        </div>
    );
}