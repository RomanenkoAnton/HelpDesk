<template>
  <header class="navbar">
    <div class="title"></div>
    <div class="user">Администратор</div>
  </header>
</template>

<style scoped>
.navbar {
  height: 52px;
  background: #fff;
  border-bottom: 1px solid #ddd;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 16px;
  flex-shrink: 0;
}
.title {
  font-weight: 600;
}
.user {
  font-size: 14px;
  color: #555;
}
</style>
