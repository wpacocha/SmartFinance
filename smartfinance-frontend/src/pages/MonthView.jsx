import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";


export default function MonthView() {
    const { year, month } = useParams();
    const [transactions, setTransactions] = useState([]);
    const [categories, setCategories] = useState([]);
    const [preferredCurrency, setPreferredCurrency] = useState("PLN");
    const [showForm, setShowForm] = useState(false);
    const [editId, setEditId] = useState(null);
    const [editForm, setEditForm] = useState({});
    const [filterType, setFilterType] = useState("all"); // 'all', 'income', 'expense'
    const [report, setReport] = useState(null);



    const [form, setForm] = useState({
        description: "",
        amount: "",
        date: new Date().toISOString().split("T")[0],
        categoryId: "",
        isIncome: false
    });

    useEffect(() => {
        const fetchAll = async () => {
            const token = localStorage.getItem("token");

            try {
                const res = await fetch(`http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const data = await res.json();
                const sorted = (data?.$values ?? []).sort((a, b) => {
                    const dateA = new Date(a.date ?? '1970-01-01');
                    const dateB = new Date(b.date ?? '1970-01-01');
                    return dateB.getTime() - dateA.getTime(); // najnowsze na górze
                });
                setTransactions(sorted);

            } catch (err) {
                console.error("Failed to fetch transactions", err);
            }

            try {
                const res = await fetch("http://localhost:5201/api/category", {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const data = await res.json();
                setCategories(data.$values ?? []);
            } catch (err) {
                console.error("Failed to fetch categories", err);
            }

            try {
                const res = await fetch("http://localhost:5201/api/user/me", {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const data = await res.json();
                setPreferredCurrency(data.preferredCurrency || "PLN");
            } catch (err) {
                console.error("Failed to fetch user", err);
            }
            try {
                const res = await fetch(`http://localhost:5201/api/report/monthly?month=${month}&year=${year}`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const data = await res.json();
                setReport(data);
            } catch (err) {
                console.error("Failed to fetch report", err);
            }

        };

        fetchAll();
    }, [month, year]);

    const handleAddTransaction = async (e) => {
            e.preventDefault();

            try {
                const res = await fetch("http://localhost:5201/api/transaction", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${localStorage.getItem("token")}`
                    },
                    body: JSON.stringify({
                        ...form,
                        amount: parseFloat(form.amount),
                        categoryId: parseInt(form.categoryId),
                        currency: preferredCurrency
                    })

                });

                if (!res.ok) throw new Error("Failed to add transaction");

                // Reset form
                setForm({
                    description: "",
                    amount: "",
                    date: new Date().toISOString().split("T")[0],
                    categoryId: "",
                    isIncome: false
                });
                setShowForm(false);

                // 🔁 Odswież dane z aktualnego miesiąca
                const updatedRes = await fetch(
                    `http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`,
                    {
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem("token")}`
                        }
                    }
                );
                const updatedData = await updatedRes.json();
                const sorted = (Array.isArray(updatedData) ? updatedData : updatedData?.$values ?? []).sort((a, b) => {
                    const dateA = new Date(a.date ?? '1970-01-01');
                    const dateB = new Date(b.date ?? '1970-01-01');
                    return dateB.getTime() - dateA.getTime();
                });
                setTransactions(sorted);

            } catch (err) {
                alert("Failed to add transaction.");
                console.error(err);
            }
    };

    const handleEditTransaction = (transaction) => {
        setEditId(transaction.id);
        setEditForm({
            description: transaction.description,
            amount: transaction.amount,
            date: transaction.date?.split("T")[0] || "",
            categoryId: transaction.categoryId,
            isIncome: transaction.isIncome
        });
    };

    const handleUpdateTransaction = async (e) => {
        e.preventDefault();
        try {
            const res = await fetch(`http://localhost:5201/api/transaction/${editId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                },
                body: JSON.stringify({
                    ...editForm,
                    amount: parseFloat(editForm.amount),
                    categoryId: parseInt(editForm.categoryId),
                    currency: preferredCurrency,
                    date: editForm.date
                })
            });

            if (!res.ok) throw new Error("Failed to update");

            setEditId(null);
            await refreshTransactions();
        } catch (err) {
            console.error(err);
            alert("Update failed.");
        }
    };

    const handleDeleteTransaction = async (id) => {
        if (!window.confirm("Delete this transaction?")) return;

        try {
            const res = await fetch(`http://localhost:5201/api/transaction/${id}`, {
                method: "DELETE",
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            });
            if (!res.ok) throw new Error("Delete failed");
            await refreshTransactions();
        } catch (err) {
            console.error(err);
            alert("Failed to delete transaction.");
        }
    };

    const refreshTransactions = async () => {
        const updatedRes = await fetch(`http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        const updatedData = await updatedRes.json();
        const sorted = (Array.isArray(updatedData) ? updatedData : updatedData?.$values ?? []).sort((a, b) => {
            const dateA = new Date(a.date ?? '1970-01-01');
            const dateB = new Date(b.date ?? '1970-01-01');
            return dateB.getTime() - dateA.getTime();
        });
        setTransactions(sorted);
    };

    return (
        <div id="month-view">
            <h2>Transactions for {new Date(year, month - 1).toLocaleString("default", { month: "long" })} {year}</h2>

            <form onSubmit={handleAddTransaction} style={{ marginBottom: "20px" }}>
                <input
                    placeholder="Description"
                    value={form.description}
                    onChange={(e) => setForm({ ...form, description: e.target.value })}
                    required
                />
                <input
                    type="number"
                    placeholder="Amount"
                    value={form.amount}
                    onChange={(e) => setForm({ ...form, amount: e.target.value })}
                    required
                />
                <input
                    type="date"
                    value={form.date}
                    onChange={(e) => setForm({ ...form, date: e.target.value })}
                    required
                />
                <select
                    value={form.categoryId}
                    onChange={(e) => setForm({ ...form, categoryId: e.target.value })}
                    required
                >
                    <option value="">-- Select Category --</option>
                    {categories.map((cat) => (
                        <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                </select>
                <label>
                    <input
                        type="checkbox"
                        checked={form.isIncome}
                        onChange={(e) => setForm({ ...form, isIncome: e.target.checked })}
                    /> Income
                </label>
                <button type="submit">Add</button>
            </form>
            <label style={{ display: 'block', marginBottom: '10px' }}>
                Show:{" "}
                <select value={filterType} onChange={(e) => setFilterType(e.target.value)}>
                    <option value="all">All</option>
                    <option value="income">Income</option>
                    <option value="expense">Expense</option>
                </select>
            </label>
            {report && (
                <div style={{ marginTop: "30px", padding: "1rem", borderTop: "1px solid #ccc" }}>
                    <h3>Monthly Report</h3>
                    <p><strong>Income:</strong> {report.income} {preferredCurrency}</p>
                    <p><strong>Expenses:</strong> {report.expenses} {preferredCurrency}</p>
                    <p><strong>Balance:</strong> {report.balance} {preferredCurrency}</p>
                   
                    <button onClick={() => {
                        const token = localStorage.getItem("token");
                        const url = `http://localhost:5201/api/report/export?month=${month}&year=${year}`;
                        fetch(url, {
                            headers: { Authorization: `Bearer ${token}` }
                        })
                            .then(res => res.blob())
                            .then(blob => {
                                const url = window.URL.createObjectURL(blob);
                                const a = document.createElement("a");
                                a.href = url;
                                a.download = `transactions_${year}_${month}.csv`;
                                a.click();
                            });
                    }}>
                        Export to CSV
                    </button>
                </div>
            )}

            <ul id="transaction-list">
                {Array.isArray(transactions) && transactions.length > 0 ? (
                    transactions
                        .filter(t => {
                            if (filterType === "income") return t.isIncome;
                            if (filterType === "expense") return !t.isIncome;
                            return true;
                        })
                        .map((t) => (
                        <li key={t.id} style={{ marginBottom: "15px" }}>
                            {editId === t.id ? (
                                <form onSubmit={handleUpdateTransaction}>
                                    <input
                                        value={editForm.description}
                                        onChange={(e) => setEditForm({ ...editForm, description: e.target.value })}
                                        required
                                    />
                                    <input
                                        type="number"
                                        value={editForm.amount}
                                        onChange={(e) => setEditForm({ ...editForm, amount: e.target.value })}
                                        required
                                    />
                                    <input
                                        type="date"
                                        value={editForm.date}
                                        onChange={(e) => setEditForm({ ...editForm, date: e.target.value })}
                                        required
                                    />
                                    <select
                                        value={editForm.categoryId}
                                        onChange={(e) => setEditForm({ ...editForm, categoryId: e.target.value })}
                                        required
                                    >
                                        <option value="">-- Select Category --</option>
                                        {categories.map((cat) => (
                                            <option key={cat.id} value={cat.id}>{cat.name}</option>
                                        ))}
                                    </select>
                                    <label>
                                        <input
                                            type="checkbox"
                                            checked={editForm.isIncome}
                                            onChange={(e) => setEditForm({ ...editForm, isIncome: e.target.checked })}
                                        /> Income
                                    </label>
                                    <button type="submit">Save</button>
                                    <button type="button" onClick={() => setEditId(null)}>Cancel</button>
                                </form>
                            ) : (
                                <div>
                                    <strong>{t.description}</strong> – {t.amount} {t.currency}
                                    <br />
                                    <small>
                                        {t.date?.split('T')[0] || 'No date'} | {t.category?.name} |{' '}
                                        {t.isIncome ? 'Income' : 'Expense'}
                                    </small>
                                    <br />
                                    <button onClick={() => handleEditTransaction(t)}>Edit</button>
                                    <button onClick={() => handleDeleteTransaction(t.id)}>Delete</button>
                                </div>
                            )}
                        </li>
                    ))

                ) : (
                    <p style={{ padding: "1rem" }}>No transactions yet. Use the + button to add one!</p>
                )}
            </ul>
        </div>

    );

}
