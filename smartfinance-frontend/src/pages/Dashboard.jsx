import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getMonths, createMonth, deleteMonth } from '../services/monthApi';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "./Dashboard.css";
import { FaFileCsv } from "react-icons/fa";


export default function Dashboard() {
    const [months, setMonths] = useState([]);
    const [showForm, setShowForm] = useState(false);
    const [yearInput, setYearInput] = useState('');
    const [monthInput, setMonthInput] = useState('');
    const navigate = useNavigate();
    const [yearlyReport, setYearlyReport] = useState(null);
    const [selectedDate, setSelectedDate] = useState(new Date());
    const [preferredCurrency, setPreferredCurrency] = useState("PLN");
    const [availableYears, setAvailableYears] = useState([]);
    const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
   
    const fetchYearlyReport = async (yearToFetch) => {
        const token = localStorage.getItem("token");

        try {
            const res = await fetch(`http://localhost:5201/api/report/yearly?year=${yearToFetch}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            const data = await res.json();
            setYearlyReport(data);
        } catch (err) {
            console.error("Failed to fetch yearly report", err);
        }
    };

    useEffect(() => {
        const fetchAvailableYears = async () => {
            const token = localStorage.getItem("token");

            try {
                const res = await fetch("http://localhost:5201/api/report/available-years", {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const years = await res.json();
                setAvailableYears(years?.$values ?? years ?? []);

                if (years.includes(new Date().getFullYear())) {
                    setSelectedYear(new Date().getFullYear());
                    fetchYearlyReport(new Date().getFullYear());
                } else if (years.length > 0) {
                    setSelectedYear(years[0]);
                    fetchYearlyReport(years[0]);
                }
            } catch (err) {
                console.error("Failed to fetch avaiable years.", err);
            }
        };
        fetchAvailableYears();
    }, []);

    useEffect(() => {
        if (selectedYear) {
            fetchYearlyReport(selectedYear);
        }
    }, [selectedYear]);


    useEffect(() => {
        const token = localStorage.getItem("token");
        fetch("http://localhost:5201/api/user/me", {
            headers: { Authorization: `Bearer ${token}` }
        })
            .then(res => res.json())
            .then(data => setPreferredCurrency(data.preferredCurrency || "PLN"))
            .catch(err => console.error("Failed to fetch user currency", err));
    }, []);

    const fetchMonths = async () => {
        try {
            const token = localStorage.getItem('token');
            const data = await getMonths(token);
            const monthsList = data?.$values ?? data ?? [];

            const sorted = monthsList.sort((a, b) =>
                b.year !== a.year
                    ? b.year - a.year
                    : b.month - a.month
            );

            setMonths(sorted);

        } catch (err) {
            console.error('Error fetching months', err);
        }
    };

    useEffect(() => {
        fetchMonths();
    }, []);

    const handleAddMonth = async (e) => {
        e.preventDefault();

        const year = parseInt(yearInput);
        const month = parseInt(monthInput);

        if (!year || !month || month < 1 || month > 12) {
            alert("Please select a valid year and month.");
            return;
        }

        try {
            const token = localStorage.getItem('token');
            await createMonth(token, year, month);
            setYearInput('');
            setMonthInput('');
            setShowForm(false);
            await fetchMonths();
            navigate(`/month/${year}/${month}`);
        } catch (err) {
            console.error('Error creating month', err);
        }

    };


    const handleDeleteMonth = async (id) => {
        if (!window.confirm('Delete this month?')) return;
        try {
            const token = localStorage.getItem('token');
            await deleteMonth(token, id);
            fetchMonths();
        } catch (err) {
            console.error('Error deleting month', err);
        }
    };

    const monthNames = [
        '', 'January', 'February', 'March',
        'April', 'May', 'June', 'July',
        'August', 'September', 'October',
        'November', 'December'
    ];

    return (
        <div id="dashboard">
            <h2>Your Available Months</h2>

            {months.length === 0 && (
                <p>No months yet. Add a transaction to get started.</p>
            )}

            <button onClick={() => setShowForm(!showForm)}>+ Add Month</button>

            {showForm && (
                <form onSubmit={handleAddMonth}>
                    <select
                        value={yearInput}
                        onChange={(e) => setYearInput(e.target.value)}
                        required
                    >
                        <option value="">-- Choose Year --</option>
                        {[...Array(11)].map((_, i) => {
                            const y = 2020 + i;
                            return <option key={y} value={y}>{y}</option>;
                        })}
                    </select>

                    <select
                        value={monthInput}
                        onChange={(e) => setMonthInput(e.target.value)}
                        required
                    >
                        <option value="">-- Choose Month --</option>
                        {monthNames.slice(1).map((name, idx) => (
                            <option key={idx + 1} value={idx + 1}>{name}</option>
                        ))}
                    </select>

                    <button type="submit">Create</button>
                </form>
            )}

            {months.map((m) => (
                <div key={m.id} className="month-box">
                    <button className="view-btn" onClick={() => navigate(`/month/${m.year}/${m.month}`)}>
                        {monthNames[m.month]} {m.year}
                    </button>
                    <div style={{display: 'flex'} }>
                    <button
                        className="charts-btn"
                        onClick={() => navigate(`/charts/${m.year}/${m.month}`)}
                    >
                        Charts
                    </button>

                    <button className="delete-btn" onClick={() => handleDeleteMonth(m.id)}>Delete</button>
                    </div>
                </div>
            ))}
            {yearlyReport && (
                <div style={{ marginTop: "30px", padding: "1rem", borderTop: "1px solid #ccc" }}>
                    <div className="year-select">
                        <label htmlFor="yearSelect" className="year-label">
                            Select year:
                        </label>
                        <select
                            id="yearSelect"
                            value={selectedYear}
                            onChange={(e) => setSelectedYear(Number(e.target.value))}
                            className="year-dropdown"
                        >
                            {availableYears.map((year) => (
                                <option key={year} value={year}>{year}</option>
                            ))}
                        </select>
                    </div>

                    <h3>Yearly Report {yearlyReport.year}</h3>
                    <p><strong>Income:</strong> {yearlyReport.income} {preferredCurrency}</p>
                    <p><strong>Expenses:</strong> {yearlyReport.expenses} {preferredCurrency}</p>
                    <p><strong>Balance:</strong> {yearlyReport.balance} {preferredCurrency}</p>

                    <button
                        className="export-btn"
                        onClick={() => {
                            const token = localStorage.getItem("token");
                            const url = `http://localhost:5201/api/report/export-yearly?year=${selectedYear}`;
                            fetch(url, {
                                headers: { Authorization: `Bearer ${token}` }
                            })
                                .then(res => res.blob())
                                .then(blob => {
                                    const url = window.URL.createObjectURL(blob);
                                    const a = document.createElement("a");
                                    a.href = url;
                                    a.download = `transactions_${selectedYear}.csv`;
                                    a.click();
                                });
                        }}
                    >
                        <FaFileCsv />
                        Export yearly CSV
                    </button>
                </div>
            )}


        </div>
    );
};
