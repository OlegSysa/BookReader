// import { useState } from "react";
import "./mainPageStyles.css";

export default function MainPage() {
    // const [token, setToken] = useState<string>("");


    const handleRegisterClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        const getToken = async () => {
            // const result = await register("", "");
            // setToken(result.data);
            console.log(event);
            console.log('test');
        };
        getToken();
        return;
    };

    return (<main className="main-page" >
        <header className="header">
            <div className="logo">
                BookReader
            </div>

            <nav className="auth-buttons">
                <button className="login-button">
                    Login1
                </button>

                <button onClick={handleRegisterClick} className="register-button">
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