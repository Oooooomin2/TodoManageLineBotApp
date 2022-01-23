export const commonData = {
    data() {
        return {
            baseUrl: "APIエンドポイント",
            status: {
                undo: '未',
                complete: '完'
            }
        }
    },
    methods: {
        changeDateToStr(target) {
            const year = target.getFullYear();
            const month = ("0" + (target.getMonth() + 1)).slice(-2);
            const date = ("0" + target.getDate()).slice(-2);
            return `${year}-${month}-${date}`;
        }
    }
}