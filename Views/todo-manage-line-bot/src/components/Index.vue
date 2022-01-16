<template>
  <v-container>
    <v-row>
      <v-col>
        <div class="my-2" v-for="todo in todos" :key="todo.Title">
          <v-row class="my-1">
            <v-col cols="10" class="my-2 text-sm">{{ todo.Title }}</v-col>
            <v-col cols="1">
              <v-btn
                block
                fab
                small
                :dark="todo.Status !== '完'"
                class="red darken-3"
                @click="completeTodo(todo.Title)"
                :disabled="todo.Status === '完'"
                ><v-icon v-if="todo.Status === '完'"
                  >mdi-check-bold</v-icon
                ><v-icon v-if="todo.Status !== '完'">mdi-weight-lifter</v-icon></v-btn
              >
            </v-col>
          </v-row>
          <v-divider></v-divider>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from "axios";

export default {
  name: "Index",
  props: ["todos"],
  data() {
    return {
      baseUrl: "APIエンドポイント",
    };
  },
  created() {
    let vm = this;
    axios.get(`${this.baseUrl}/api/todos`).then(function (response) {
      response.data.forEach((element) => {
        vm.todos.push({
          Title: element.Title,
          Status: element.Status,
        });
      });
    });
  },
  methods: {
    completeTodo: function (title) {
      const todo = {
        Title: title,
        Status: "完",
        ImplementationDate: new Date(
          Date.now() - new Date().getTimezoneOffset() * 60000
        )
          .toISOString()
          .substr(0, 10),
      };

      let vm = this;
      axios
        .put(`${this.baseUrl}/api/todos/`, todo)
        .then(function () {
          vm.todos.map(function (v) {
            if (v.Title === title) {
              v.Status = "完";
            }
          });
        })
        .catch(function (response) {
          console.log(response);
        });
    },
  },
};
</script>
