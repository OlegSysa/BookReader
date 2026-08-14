import { useState } from "react";
import { Register, googleAuth } from "../../api/Auth";
import { useNavigate } from "react-router-dom";
import "./AuthStyles.css";

interface RegisterFormProps {
    onClose: () => void;
}

export default function RegisterForm({ onClose }: RegisterFormProps) {

    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [isExternalLoading, setIsExternalLoading] = useState(false);
    const navigate = useNavigate();

    const handleRegisterButtonClick = async () => {
        setIsLoading(true);
        try {
            const result = await Register(login, password);
            if (result.success) {
                onClose();
                navigate("/dashboard");
            }
            else {
                setError(result.errorMessage ?? "Failed to register");
            }
        }
        catch {
            setError("Failed to register");
        }
        finally {
            setIsLoading(false);
        }
    };

    const handleGoogleRegister = () => {
        setIsExternalLoading(true);
        googleAuth("register");
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
                <h2>Register</h2>
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

            <button onClick={handleRegisterButtonClick}
                type="button"
                disabled={isLoading}>
                {isLoading ? (
                    <span className="spinner"></span>
                ) : (
                    "Register"
                )}
            </button>
            <button
                type="button"
                onClick={handleGoogleRegister}
                disabled={isExternalLoading}
            >
                {isExternalLoading ? (
                    <span className="spinner"></span>
                ) : (
                    "Continue with Google"
                )}

            </button>
        </div>
    );
}