import { useEffect, useState } from "react";

function App() {
  const [status, setStatus] = useState("Loading...");

  useEffect(() => {
    fetch("https://localhost:7266/api/health") // замени на свой порт
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
    </div>
  );
}

export default App;