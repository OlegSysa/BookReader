import { useEffect, useState } from "react";
import Chapter from "./components/chapter/chapter";

function App() {
  const [status, setStatus] = useState("Loading...");

  useEffect(() => {
    fetch(`${import.meta.env.VITE_API_BASE_URL}/health`)
      .then((response) => response.json())
      .then((data) => setStatus(data.status))
      .catch((error) => {
        console.error(error);
        setStatus("Backend unavailable");
      });
  }, []);

  return (
    <div style={{ padding: "2rem" }}>
      <h1>BookReader</h1>
      <p>Backend status: {status}</p>
      <div>
        <Chapter />
      </div>
    </div>
  );
}

export default App;