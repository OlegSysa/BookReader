import { useState } from "react";
import "./mainPageStyles.css";
import RegisterForm from "../auth/RegisterForm";
import LoginForm from "../auth/LoginForm";


export default function MainPage() {
    const [showRegister, setShowRegister] = useState(false);
    const [showLogin, setShowLogin] = useState(false);


    const handleRegisterClick = () => {
        setShowRegister(true);
        setShowLogin(false);
    };

    const handleLoginClick = () => {
        setShowLogin(true);
        setShowRegister(false);
    };

    return (<main className="main-page" >
        <header className="header">

            {showRegister && (
                <div className="auth-form-container">
                    <RegisterForm onClose={() => setShowRegister(false)} />
                </div>
            )}
            {showLogin && (
                <div className="auth-form-container">
                    <LoginForm onClose={() => setShowLogin(false)} />
                </div>
            )}
            {/* <div className="logo">
                BookReader
            </div> */}

            <nav className="auth-links">
                <button onClick={handleLoginClick}>
                    Login
                </button>

                <button onClick={handleRegisterClick}>
                    Register
                </button>
            </nav>
        </header>

        <section className="hero">
            <h1>Read. Learn. Discover.</h1>

            <p>
                Your books, your words, your knowledge.
            </p>
        </section>
    </main>);
}