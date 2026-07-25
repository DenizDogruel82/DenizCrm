const token = sessionStorage.getItem("accessToken");
const notification = document.querySelector("#notification");
let customers = [];
let activities = [];

if (!token) {
  window.location.replace("/");
} else {
  initialize();
}

async function api(path, options = {}) {
  const response = await fetch(path, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
      ...options.headers
    }
  });

  if (response.status === 401) {
    logout();
    throw new Error("Oturum süresi doldu.");
  }
  if (!response.ok) {
    const problem = await response.json().catch(() => ({}));
    throw new Error(problem.detail || problem.title || "API isteği başarısız.");
  }
  return response.status === 204 ? null : response.json();
}

async function initialize() {
  try {
    const [user, customerResult, activityResult] = await Promise.all([
      api("/api/auth/me"),
      api("/api/customers?pageSize=100"),
      api("/api/activities")
    ]);
    const name = user.fullName || user.email || "Kullanıcı";
    document.querySelector("#userName").textContent = name;
    document.querySelector("#avatar").textContent = name.charAt(0).toLocaleUpperCase("tr-TR");
    sessionStorage.setItem("currentUser", JSON.stringify(user));
    customers = customerResult.items;
    activities = activityResult;
    renderAll();
  } catch (error) {
    notify(error.message, true);
  }
}

function renderAll() {
  document.querySelector("#customerCount").textContent = customers.length;
  document.querySelector("#activityCount").textContent = activities.length;
  renderCustomers(customers);
  renderActivities();
  document.querySelector("#activityCustomer").innerHTML =
    `<option value="">Müşteri seçilmedi</option>${customers.map(customer =>
      `<option value="${customer.id}">${escapeHtml(customer.name)}</option>`).join("")}`;
}

function renderCustomers(items) {
  const rows = document.querySelector("#customerRows");
  rows.innerHTML = items.length ? items.map(customer => `
    <tr>
      <td><strong>${escapeHtml(customer.name)}</strong></td>
      <td>${customer.type === 2 ? "Şirket" : "Bireysel"}</td>
      <td>${escapeHtml(customer.email || customer.phone || "—")}</td>
      <td>${escapeHtml(customer.city || "—")}</td>
      <td><button class="danger" data-delete-customer="${customer.id}">Sil</button></td>
    </tr>`).join("") : `<tr><td class="empty-state" colspan="5">Müşteri bulunamadı.</td></tr>`;
}

function renderActivities() {
  const typeNames = ["", "Arama", "E-posta", "Toplantı", "Görev", "Demo"];
  const statusNames = ["", "Planlandı", "Tamamlandı", "İptal"];
  document.querySelector("#activityCards").innerHTML = activities.length ? activities.map(activity => `
    <article class="activity-card">
      <div>
        <span class="badge">${typeNames[activity.type] || activity.type}</span>
        <h3>${escapeHtml(activity.subject)}</h3>
        <p>${escapeHtml(activity.description || "Açıklama yok")}</p>
        <small>${statusNames[activity.status] || activity.status}${activity.dueAtUtc ? ` · ${new Date(activity.dueAtUtc).toLocaleString("tr-TR")}` : ""}</small>
      </div>
      <div class="card-actions">
        ${activity.status === 1 ? `<button class="secondary" data-complete-activity="${activity.id}">Tamamla</button>` : ""}
        <button class="danger" data-delete-activity="${activity.id}">Sil</button>
      </div>
    </article>`).join("") : `<div class="empty-state">Aktivite bulunamadı.</div>`;
}

function notify(message, error = false) {
  notification.textContent = message;
  notification.className = `notification${error ? " error" : ""}`;
  setTimeout(() => { notification.textContent = ""; }, 4000);
}

function escapeHtml(value) {
  const element = document.createElement("span");
  element.textContent = value;
  return element.innerHTML;
}

function logout() {
  sessionStorage.removeItem("accessToken");
  sessionStorage.removeItem("currentUser");
  window.location.replace("/");
}

document.querySelector("#logoutButton").addEventListener("click", logout);

document.querySelectorAll(".nav-link").forEach(button => {
  button.addEventListener("click", () => {
    document.querySelectorAll(".nav-link, .view").forEach(element => element.classList.remove("active"));
    button.classList.add("active");
    document.querySelector(`#${button.dataset.view}View`).classList.add("active");
  });
});

document.querySelector("#showCustomerForm").addEventListener("click", () => document.querySelector("#customerForm").classList.remove("hidden"));
document.querySelector("#cancelCustomer").addEventListener("click", () => document.querySelector("#customerForm").classList.add("hidden"));
document.querySelector("#showActivityForm").addEventListener("click", () => document.querySelector("#activityForm").classList.remove("hidden"));
document.querySelector("#cancelActivity").addEventListener("click", () => document.querySelector("#activityForm").classList.add("hidden"));

document.querySelector("#customerSearch").addEventListener("input", event => {
  const search = event.target.value.toLocaleLowerCase("tr-TR");
  renderCustomers(customers.filter(customer =>
    [customer.name, customer.email, customer.city].some(value => value?.toLocaleLowerCase("tr-TR").includes(search))));
});

document.querySelector("#customerForm").addEventListener("submit", async event => {
  event.preventDefault();
  const form = new FormData(event.currentTarget);
  try {
    const customer = await api("/api/customers", {
      method: "POST",
      body: JSON.stringify({
        name: form.get("name"), type: Number(form.get("type")),
        email: form.get("email") || null, phone: form.get("phone") || null,
        industry: form.get("industry") || null, city: form.get("city") || null,
        website: null, taxNumber: null, address: null, country: null, ownerUserId: null
      })
    });
    customers.unshift(customer);
    event.currentTarget.reset();
    event.currentTarget.classList.add("hidden");
    renderAll();
    notify("Müşteri başarıyla eklendi.");
  } catch (error) { notify(error.message, true); }
});

document.querySelector("#activityForm").addEventListener("submit", async event => {
  event.preventDefault();
  const form = new FormData(event.currentTarget);
  try {
    const activity = await api("/api/activities", {
      method: "POST",
      body: JSON.stringify({
        subject: form.get("subject"), description: form.get("description") || null,
        type: Number(form.get("type")), customerId: form.get("customerId") || null,
        contactId: null, leadId: null, opportunityId: null, assignedUserId: null,
        dueAtUtc: form.get("dueAtUtc") ? new Date(form.get("dueAtUtc")).toISOString() : null
      })
    });
    activities.unshift(activity);
    event.currentTarget.reset();
    event.currentTarget.classList.add("hidden");
    renderAll();
    notify("Aktivite başarıyla eklendi.");
  } catch (error) { notify(error.message, true); }
});

document.addEventListener("click", async event => {
  const customerId = event.target.dataset.deleteCustomer;
  const activityId = event.target.dataset.deleteActivity;
  const completeId = event.target.dataset.completeActivity;
  try {
    if (customerId) {
      await api(`/api/customers/${customerId}`, { method: "DELETE" });
      customers = customers.filter(customer => customer.id !== customerId);
      notify("Müşteri silindi.");
    } else if (activityId) {
      await api(`/api/activities/${activityId}`, { method: "DELETE" });
      activities = activities.filter(activity => activity.id !== activityId);
      notify("Aktivite silindi.");
    } else if (completeId) {
      const updated = await api(`/api/activities/${completeId}/complete`, { method: "POST" });
      activities = activities.map(activity => activity.id === completeId ? updated : activity);
      notify("Aktivite tamamlandı.");
    } else return;
    renderAll();
  } catch (error) { notify(error.message, true); }
});
