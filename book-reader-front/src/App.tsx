//import { useEffect, useState } from "react";
import { Routes, Route } from "react-router-dom";
import MainPage from "./components/main/MainPage";
import UserPage from "./components/user/UserPage";

function App() {

  // const [status, setStatus] = useState("Loading...");

  // useEffect(() => {
  //   fetch(`${import.meta.env.VITE_API_BASE_URL}/health`)
  //     .then((response) => response.json())
  //     .then((data) => {
  //       setStatus(data.status);
  //       console.log(status);
  //     })
  //     .catch((error) => {
  //       console.error(error);
  //       setStatus("Backend unavailable");
  //     });
  // }, []);

  return (
    <Routes>
      <Route path="/" element={<MainPage />} />
      <Route path="/userpage" element={<UserPage />} />
    </Routes>
  );
}

export default App;