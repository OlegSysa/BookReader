import { useEffect, useState } from "react";
import MainPage from "./components/main/mainPage";
function App() {

  const [status, setStatus] = useState("Loading...");

  useEffect(() => {
    fetch(`${import.meta.env.VITE_API_BASE_URL}/health`)
      .then((response) => response.json())
      .then((data) => {
        setStatus(data.status);
        console.log(status);
      })
      .catch((error) => {
        console.error(error);
        setStatus("Backend unavailable");
      });
  }, []);

  return (
    <div>
      <div>
        <MainPage />
      </div>
    </div>
  );
}

export default App;