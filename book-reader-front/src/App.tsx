import { Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import MainPage from "./components/main/MainPage";
import Dashboard from "./components/dashboard/Dashboard";
import BooksPage from "./components/book/BooksPage";
import ProfilePage from "./components/profile/Profile";
import WordsPage from "./components/Word/WordsPage";
import ContentContainer from "./components/content/ContentContainer/ContentContainer";

function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<MainPage />} />

        <Route path="/dashboard" element={<Dashboard />}>
          <Route index element={<BooksPage />} />
          <Route path="profile" element={<ProfilePage />} />
          <Route path="words" element={<WordsPage />} />
        </Route>
        <Route
          path="/books/:bookId"
          element={<ContentContainer />}
        />
      </Route>
    </Routes>
  );
}

export default App;