<script setup lang="ts">
import { Home, Building2, Laptop, ClipboardList } from 'lucide-vue-next'
</script>

<template>
  <aside class="sidebar">
    <div class="logo">HelpDesk</div>
    <nav>
      <RouterLink to="/" exact-active-class="active">
        <Home :size="18" /> Главная
      </RouterLink>
      <RouterLink to="/tickets" active-class="active">
        <ClipboardList :size="18" /> Заявки
      </RouterLink>
      <RouterLink to="/departments" active-class="active">
        <Building2 :size="18" /> Підрозділи
      </RouterLink>
      <RouterLink to="/units" active-class="active">
        <Laptop :size="18" /> ТМЦ
      </RouterLink>
      <!-- новые пункты меню добавляйте сюда -->
    </nav>
  </aside>
</template>

<style scoped>
.sidebar {
  width: 200px;
  background: var(--sidebar-bg);
  color: #fff;
  display: flex;
  flex-direction: column;
  flex-shrink: 0;
}
.logo {
  padding: 16px;
  font-size: 20px;
  font-weight: 700;
  border-bottom: 1px solid #333b41;
}
nav {
  display: flex;
  flex-direction: column;
}
nav a {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #c9cfd3;
  text-decoration: none;
  padding: 12px 16px;
  border-left: 3px solid transparent;
}
nav a:hover {
  background: #2a3035;
}
nav a.active {
  background: #2a3035;
  color: #fff;
  border-left-color: var(--yellow);
}
</style>
