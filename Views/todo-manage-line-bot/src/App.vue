<template>
  <v-app>
    <v-app-bar app color="red darken-3" dark>
      <div class="d-flex align-center mr-3">
        <v-toolbar-title class="text-h5 font-navigation">
          今日やることアプリ
        </v-toolbar-title>
      </div>
      <Create :todos="todos" />
    </v-app-bar>

    <v-main>
      <Index :todos="todos" />
    </v-main>
  </v-app>
</template>

<script>
import Index from "./components/Index";
import Create from "./components/Create";

export default {
  name: "App",

  components: {
    Index,
    Create,
  },
  data() {
    return {
      todos: [],
    };
  },
};
</script>

<style>
* {
  font-family: 游ゴシック, "Yu Gothic", メイリオ, Meiryo, Verdana, Helvetica,
    Arial, sans-serif !important;
}
</style>