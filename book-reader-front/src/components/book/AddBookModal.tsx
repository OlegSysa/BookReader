import { useState } from "react";
import "./AddBookModal.css";
import { uploadBook } from "../../api/Book";

interface AddBookModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function AddBookModal({
    isOpen,
    onClose
}: AddBookModalProps) {
    const [title, setTitle] = useState("");
    const [author, setAuthor] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState("");
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!file) {
            return;
        }

        try {
            setIsLoading(true);
            await uploadBook(file, title, author);
            onClose();
        } catch {
            setError("Failed to upload book");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div
            className={`modal-overlay ${isOpen ? "modal-overlay-visible" : ""}`}
            onClick={onClose}
        >
            <div
                className={`add-book-modal ${isOpen ? "add-book-modal-visible" : ""}`}
                onClick={(e) => e.stopPropagation()}
            >
                <div className="modal-header">
                    <h2>Add new book</h2>

                    <button
                        type="button"
                        className="modal-close"
                        onClick={onClose}
                    >
                        ×
                    </button>
                </div>

                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="Book title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />

                    <input
                        type="text"
                        placeholder="Author"
                        value={author}
                        onChange={(e) => setAuthor(e.target.value)}
                    />

                    <input
                        type="file"
                        accept=".epub,.pdf"
                        onChange={(e) =>
                            setFile(e.target.files?.[0] ?? null)
                        }
                    />
                    {error && <div className="add-book-error">{error}</div>}
                    <div className="modal-actions">
                        <button
                            type="button"
                            onClick={onClose}
                        >
                            Cancel
                        </button>

                        <button type="submit" disabled={!file || isLoading}>
                            {isLoading ? (
                                <span className="spinner"></span>
                            ) : (
                                "Add book"
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}