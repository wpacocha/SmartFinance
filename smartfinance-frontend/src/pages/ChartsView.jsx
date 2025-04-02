import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { Pie, Bar, Doughnut } from "react-chartjs-2";
import { Chart as ChartJS, ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement } from "chart.js";

ChartJS.register(ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement);

export default function ChartsView() {
    const { year, month } = useParams();
    const [transactions, setTransactions] = useState([]);
    const [chartType, setChartType] = useState("pie");
    const [preferredCurrency, setPreferredCurrency] = useState("PLN");

    useEffect(() => {
        const fetchData = async () => {
            const token = localStorage.getItem("token");

            try {
                const res = await fetch(`http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const data = await res.json();
                setTransactions(data?.$values ?? []);
            } catch (err) {
                console.error("Failed to fetch chart data", err);
            }

            try {
                const res = await fetch("http://localhost:5201/api/user/me", {
                    headers: { Authorization: `Bearer ${token}` }
                });
                const user = await res.json();
                setPreferredCurrency(user.preferredCurrency || "PLN");
            } catch (err) {
                console.error("Failed to fetch user currency", err);
            }
        };

        fetchData();
    }, [month, year]);

    const getChartData = (isIncome) => {
        const filtered = transactions.filter(t => t.isIncome === isIncome);

        const sumsByCategory = {};
        filtered.forEach((t) => {
            const category = t.category?.name || "Other";
            if (!sumsByCategory[category]) sumsByCategory[category] = 0;
            sumsByCategory[category] += t.amount;
        });

        const labels = Object.keys(sumsByCategory);
        const data = Object.values(sumsByCategory);

        return {
            labels,
            datasets: [
                {
                    label: isIncome ? "Income" : "Expenses",
                    data,
                    backgroundColor: [
                        "#4caf50", "#f44336", "#2196f3", "#ff9800", "#9c27b0", "#03a9f4"
                    ],
                    borderWidth: 1
                }
            ]
        };
    };

    const renderChart = (data) => {
        switch (chartType) {
            case "pie": return <Pie data={data} />;
            case "bar": return <Bar data={data} />;
            case "doughnut": return <Doughnut data={data} />;
            default: return <Pie data={data} />;
        }
    };

    return (
        <div style={{ maxWidth: "800px", margin: "0 auto", padding: "30px" }}>
            <h2 style={{ marginBottom: "20px" }}>
                Charts for {new Date(year, month - 1).toLocaleString("en-US", { month: "long" })} {year}
            </h2>

            <div style={{ marginBottom: "20px" }}>
                <label htmlFor="chart-type">Chart type:</label>
                <select
                    id="chart-type"
                    value={chartType}
                    onChange={(e) => setChartType(e.target.value)}
                    style={{ marginLeft: "10px" }}
                >
                    <option value="pie">Pie</option>
                    <option value="bar">Bar</option>
                    <option value="doughnut">Doughnut</option>
                </select>
            </div>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "40px" }}>
                <div>
                    <h4 style={{ textAlign: "center", marginBottom: "10px" }}>Income</h4>
                    {renderChart(getChartData(true))}
                </div>
                <div>
                    <h4 style={{ textAlign: "center", marginBottom: "10px" }}>Expenses</h4>
                    {renderChart(getChartData(false))}
                </div>
            </div>
        </div>
    );
}
