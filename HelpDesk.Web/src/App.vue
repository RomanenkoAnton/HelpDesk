<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppSidebar from '@/components/AppSidebar.vue'
import AppNavbar from '@/components/AppNavbar.vue'
</script>

<template>
  <div class="layout">
    <AppSidebar />
    <div class="main">
      <AppNavbar />
      <div class="content">
        <RouterView />
      </div>
    </div>
  </div>
</template>

<style>
:root {
  --yellow: #f6d20a;
  --yellow-soft: #fff1a6;
  --green-row: #cdf5a3;
  --green-row-alt: #dcf8bb;
  --header-bg: #e4e4e4;
  --border: #cfcfcf;
  --sidebar-bg: #1f2428;
  --text: #222;
}

* {
  box-sizing: border-box;
}
body {
  margin: 0;
  font-family: system-ui, sans-serif;
  font-size: 14px;
  background: #fff;
  color: var(--text);
}

/* кнопки */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border: 1px solid var(--border);
  border-radius: 4px;
  background: #fff;
  color: var(--text);
  font-size: 14px;
  cursor: pointer;
}
.btn:hover {
  background: #f4f4f4;
}
.btn-primary {
  background: var(--yellow);
  border-color: var(--yellow);
  font-weight: 600;
}
.btn-primary:hover {
  background: #e6c200;
}
.btn:disabled {
  opacity: 0.5;
  cursor: default;
}

/* поля ввода */
.field {
  width: 100%;
  padding: 5px 8px;
  border: 1px solid var(--border);
  border-radius: 3px;
  background: #fff;
  font-size: 14px;
}
.field:focus {
  outline: none;
  border-color: var(--yellow);
  box-shadow: 0 0 0 2px var(--yellow-soft);
}
</style>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
}
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}
.content {
  flex: 1;
  overflow: auto;
  padding: 0;
}
</style>
