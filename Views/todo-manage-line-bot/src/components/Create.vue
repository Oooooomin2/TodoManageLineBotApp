<template>
  <v-dialog
    transition="dialog-bottom-transition"
    max-width="600"
    v-model="dialog"
  >
    <template v-slot:activator="{ on, attrs }">
      <v-btn
        class="ma-2 font-navigation"
        light
        x-small
        color="white"
        fab
        v-bind="attrs"
        v-on="on"
        @click="dialog = true"
        ><v-icon>mdi-plus</v-icon>
      </v-btn>
    </template>
    <template>
      <v-card>
        <v-toolbar class="text-h6" color="red darken-3" dark
          >Todo登録</v-toolbar
        >
        <v-card-text>
          <v-container>
            <form>
              <v-row class="text-center">
                <v-col>
                  <v-row>
                    <v-col cols="12">
                      <v-menu
                        v-model="menu"
                        :close-on-content-click="false"
                        transition="scale-transition"
                        offset-y
                        min-width="auto"
                      >
                        <template v-slot:activator="{ on, attrs }">
                          <v-text-field
                            v-model="implementationDate"
                            label="日付"
                            prepend-icon="mdi-calendar"
                            readonly
                            v-bind="attrs"
                            v-on="on"
                          ></v-text-field>
                        </template>
                        <v-date-picker
                          locale="ja"
                          v-model="implementationDate"
                          @input="menu = false"
                          :min="implementationDate"
                          :max="limitImplementationDate"
                          :day-format="(date) => new Date(date).getDate()"
                        ></v-date-picker>
                      </v-menu>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field
                        class="font-weight-bold text-h6 mt-4"
                        label="タスクを入力してください"
                        v-model="title"
                        persistent-hint
                        outlined
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12">
                      <v-btn block @click="postTodo">登録</v-btn>
                    </v-col>
                  </v-row>
                </v-col>
              </v-row>
            </form>
          </v-container>
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn text @click="dialog = false">Close</v-btn>
        </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>

<script>
import axios from "axios";

export default {
  name: "Create",
  props: ["todos"],
  data: () => ({
    baseUrl: "APIエンドポイント",
    implementationDate: "",
    menu: false,
    title: "",
    dialog: false,
    limitImplementationDate: ''
  }),
  created() {
    const today = new Date();
    const year = today.getFullYear();
    const month = ("0" + (today.getMonth() + 1)).slice(-2);
    const date = ("0" + today.getDate()).slice(-2);
    this.implementationDate = `${year}-${month}-${date}`;

    const limitDate = new Date(
      new Date().setDate(new Date().getDate() + 7)
    );

    const limitDateYear = limitDate.getFullYear();
    const limitDateMonth = ("0" + (limitDate.getMonth() + 1)).slice(-2);
    const limitDateDate = ("0" + limitDate.getDate()).slice(-2);
    this.limitImplementationDate = `${limitDateYear}-${limitDateMonth}-${limitDateDate}`;
  },
  methods: {
    postTodo() {
      const todo = {
        Title: this.title,
        Status: "未",
        ImplementationDate: this.implementationDate,
      };

      let vm = this;
      axios
        .post(`${this.baseUrl}/api/todos`, todo)
        .then(function () {
          const today = new Date();
          const implementationDate = new Date(todo.ImplementationDate);
          if (
            today.getFullYear() === implementationDate.getFullYear() &&
            today.getMonth() === implementationDate.getMonth() &&
            today.getDate() === implementationDate.getDate()
          ) {
            vm.todos.push(todo);
          }
          vm.dialog = false;
          vm.title = "";
        })
        .catch(function (response) {
          console.log(response);
        });
    },
  },
};
</script>
