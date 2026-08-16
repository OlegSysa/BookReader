import { useParams } from "react-router-dom";
import Chapter from "../chapter";

export default function ChapterPage() {
    const { bookId } = useParams();

    if (!bookId) {
        return <div>Book not found</div>;
    }

    return <Chapter bookId={Number(bookId)} />;
}