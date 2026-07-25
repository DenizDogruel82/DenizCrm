const token = sessionStorage.getItem("accessToken");
const state = { customers: [], activities: [] };

if (!token) window.location.replace("/");

const api = async (path, options = {}) => {
  const response = await fetch(path, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
      ...options.headers
    }
  });
  if (response.status === 401) return clearSessionAndRedirect();
  if (response.status === 204) return null;
  const body = await response.json();
  if (!response.ok) {
    const validation = body.errors
      ? Object.values(body.errors).flat().join(" ")
      : body.detail;
    throw new Error(validation || "İşlem tamamlanamadı.");
  }
  return body;
};

document.addEventListener("DOMContentLoaded", initialize);

async function initialize() {
  bindEvents();
  try {
    const user = await api("/api/auth/me");
    const name = user.fullName || "Kullanıcı";
    document.querySelector("#userName").textContent = name;
    document.querySelector("#avatar").textContent = name[0].toLocaleUpperCase("tr-TR");
    await loadDashboard();
  } catch (error) {
    notify(error.message, true);
  }
}

function bindEvents() {
  document.querySelectorAll(".nav-link").forEach(button =>
    button.addEventListener("click", () => switchView(button.dataset.view)));
  document.querySelectorAll("[data-go]").forEach(button =>
    button.addEventListener("click", () => switchView(button.dataset.go)));
  document.querySelector("#logoutButton").addEventListener("click", clearSessionAndRedirect);
  document.querySelector("#showCustomerForm").addEventListener("click", () => toggleForm("customerForm", true));
  document.querySelector("#showActivityForm").addEventListener("click", () => toggleForm("activityForm", true));
  document.querySelectorAll(".cancel-form").forEach(button =>
    button.addEventListener("click", () => button.closest("form").classList.add("hidden")));
  document.querySelector("#customerForm").addEventListener("submit", createCustomer);
  document.querySelector("#activityForm").addEventListener("submit", createActivity);
  document.querySelector("#customerSearch").addEventListener("input", event =>
    renderCustomers(state.customers.filter(customer =>
      customer.name.toLocaleLowerCase("tr-TR").includes(
        event.target.value.toLocaleLowerCase("tr-TR")))));
}

async function loadDashboard() {
  const [customers, opportunities, activities, insights] = await Promise.all([
    api("/api/customers?pageSize=100"),
    api("/api/opportunities?pageSize=100"),
    api("/api/activities"),
    api("/api/ai-insights")
  ]);
  state.customers = customers.items;
  state.activities = activities;
  document.querySelector("#customerCount").textContent = customers.totalCount;
  document.querySelector("#opportunityCount").textContent = opportunities.totalCount;
  document.querySelector("#activityCount").textContent = activities.length;
  document.querySelector("#insightCount").textContent = insights.length;
  renderRecentCustomers();
  renderCustomers(state.customers);
  renderActivities();
  fillCustomerSelect();
}

async function switchView(view) {
  const meta = {
    overview: ["CRM ANA SAYFASI", `Merhaba, ${document.querySelector("#userName").textContent}`, "Satış operasyonunuzun bugünkü görünümü."],
    customers: ["MÜŞTERİ YÖNETİMİ", "Müşteriler", "Şirketleri ve iletişim bilgilerini yönetin."],
    activities: ["SATIŞ TAKİBİ", "Aktiviteler", "Arama, toplantı ve görevlerinizi planlayın."]
  };
  document.querySelectorAll(".view").forEach(element => element.classList.remove("active"));
  document.querySelector(`#${view}View`).classList.add("active");
  document.querySelectorAll(".nav-link").forEach(button =>
    button.classList.toggle("active", button.dataset.view === view));
  document.querySelector("#pageEyebrow").textContent = meta[view][0];
  document.querySelector("#pageTitle").textContent = meta[view][1];
  document.querySelector("#pageDescription").textContent = meta[view][2];
}

async function createCustomer(event) {
  event.preventDefault();
  const form = new FormData(event.target);
  try {
    await api("/api/customers", {
      method: "POST",
      body: JSON.stringify({
        name: form.get("name"), type: 2, email: emptyToNull(form.get("email")),
        phone: emptyToNull(form.get("phone")), website: null,
        industry: emptyToNull(form.get("industry")), taxNumber: null, address: null,
        city: emptyToNull(form.get("city")), country: emptyToNull(form.get("country")),
        ownerUserId: null
      })
    });
    event.target.reset();
    toggleForm("customerForm", false);
    notify("Müşteri başarıyla eklendi.");
    await loadDashboard();
  } catch (error) { notify(error.message, true); }
}

async function deleteCustomer(id) {
  if (!confirm("Müşteri silinsin mi?")) return;
  try {
    await api(`/api/customers/${id}`, { method: "DELETE" });
    notify("Müşteri silindi.");
    await loadDashboard();
  } catch (error) { notify(error.message, true); }
}

async function createActivity(event) {
  event.preventDefault();
  const form = new FormData(event.target);
  try {
    await api("/api/activities", {
      method: "POST",
      body: JSON.stringify({
        subject: form.get("subject"), description: emptyToNull(form.get("description")),
        type: Number(form.get("type")), customerId: form.get("customerId"),
        contactId: null, leadId: null, opportunityId: null, assignedUserId: null,
        dueAtUtc: new Date(form.get("dueAtUtc")).toISOString()
      })
    });
    event.target.reset();
    toggleForm("activityForm", false);
    notify("Aktivite planlandı.");
    await loadDashboard();
  } catch (error) { notify(error.message, true); }
}

async function completeActivity(id) {
  try {
    await api(`/api/activities/${id}/complete`, { method: "POST" });
    notify("Aktivite tamamlandı.");
    await loadDashboard();
  } catch (error) { notify(error.message, true); }
}

function renderRecentCustomers() {
  const root = document.querySelector("#recentCustomers");
  root.innerHTML = state.customers.slice(0, 5).map(customer => `
    <div class="list-row"><span class="mini-avatar">${escapeHtml(customer.name[0])}</span>
      <div><strong>${escapeHtml(customer.name)}</strong><small>${escapeHtml(customer.industry || "Sektör belirtilmedi")}</small></div>
      <span>${escapeHtml(customer.city || "—")}</span></div>`).join("")
    || '<div class="empty-state">İlk müşterinizi ekleyin.</div>';
}

function renderCustomers(customers) {
  document.querySelector("#customersTable").innerHTML = customers.map(customer => `
    <tr><td><strong>${escapeHtml(customer.name)}</strong><small>${escapeHtml(customer.email || "—")}</small></td>
      <td>${escapeHtml(customer.phone || "—")}</td><td>${escapeHtml(customer.industry || "—")}</td>
      <td>${escapeHtml([customer.city, customer.country].filter(Boolean).join(", ") || "—")}</td>
      <td><button class="danger" onclick="deleteCustomer('${customer.id}')">Sil</button></td></tr>`).join("");
  document.querySelector("#customersEmpty").classList.toggle("hidden", customers.length > 0);
}

function renderActivities() {
  const names = Object.fromEntries(state.customers.map(customer => [customer.id, customer.name]));
  document.querySelector("#activitiesList").innerHTML = state.activities.map(activity => `
    <article class="activity-card"><div><span class="badge">${activityType(activity.type)}</span>
      <h3>${escapeHtml(activity.subject)}</h3><p>${escapeHtml(names[activity.customerId] || "CRM kaydı")}</p>
      <small>${formatDate(activity.dueAtUtc)}</small></div>
      ${activity.status === 1 ? `<button class="secondary" onclick="completeActivity('${activity.id}')">Tamamla</button>` : '<span class="done">Tamamlandı</span>'}
    </article>`).join("");
  document.querySelector("#activitiesEmpty").classList.toggle("hidden", state.activities.length > 0);
}

function fillCustomerSelect() {
  document.querySelector("#activityCustomer").innerHTML =
    '<option value="">Müşteri seçin</option>' +
    state.customers.map(customer =>
      `<option value="${customer.id}">${escapeHtml(customer.name)}</option>`).join("");
}

function toggleForm(id, visible) { document.querySelector(`#${id}`).classList.toggle("hidden", !visible); }
function notify(text, error = false) {
  const element = document.querySelector("#notification");
  element.textContent = text; element.classList.toggle("error", error);
  setTimeout(() => { if (element.textContent === text) element.textContent = ""; }, 3500);
}
function clearSessionAndRedirect() {
  sessionStorage.clear(); window.location.replace("/");
}
function emptyToNull(value) { return value?.trim() || null; }
function activityType(type) { return ({ 1: "Arama", 2: "E-posta", 3: "Toplantı", 4: "Görev", 5: "Demo" })[type] || "Aktivite"; }
function formatDate(value) { return value ? new Intl.DateTimeFormat("tr-TR", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value)) : "Tarih yok"; }
function escapeHtml(value) { const div = document.createElement("div"); div.textContent = value ?? ""; return div.innerHTML; }
