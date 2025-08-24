import { apiGetLeadProcess } from "@/services/contact";
import { BRANCH_OPTIONS } from "@/utils/constants";
import { ProTable } from "@ant-design/pro-components";
import { useModel } from "@umijs/max";
import { Tag } from "antd";
import dayjs from "dayjs";

const EventDashboard: React.FC = () => {
    const { initialState } = useModel('@@initialState');
    return (
        <>
            <ProTable
                rowKey="id"
                headerTitle="Lịch sử sự kiện"
                request={(params) => apiGetLeadProcess({
                    ...params,
                    branch: initialState?.currentUser?.branch
                })}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Key-in',
                        dataIndex: 'leadName',
                        width: 200
                    },
                    {
                        title: 'Số điện thoại',
                        dataIndex: 'phoneNumber',
                        search: false,
                        width: 140
                    },
                    {
                        title: 'Chi nhánh',
                        dataIndex: 'branch',
                        search: false,
                        minWidth: 100,
                        valueType: 'select',
                        fieldProps: {
                            options: BRANCH_OPTIONS
                        }
                    },
                    {
                        title: 'Nhân viên',
                        dataIndex: 'userName',
                        width: 200
                    },
                    {
                        title: 'Quyền',
                        dataIndex: 'roleName',
                        search: false,
                        width: 150
                    },
                    {
                        title: 'Thời gian',
                        dataIndex: 'createdDate',
                        search: false,
                        render: (_, entity) => entity.createdDate ? dayjs(entity.createdDate).format('DD-MM-YYYY HH:mm') : '-',
                        width: 140
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'status',
                        valueEnum: {
                            0: <Tag color="yellow" className="w-full text-center">Chờ duyệt</Tag>,
                            1: <Tag color="orange" className="w-full text-center">Đã duyệt</Tag>,
                            2: <Tag color="red" className="w-full text-center">Check-In</Tag>,
                            3: <Tag color="blue" className="w-full text-center">Chốt deal</Tag>,
                            4: <Tag color="black" className="w-full text-center">Từ chối</Tag>,
                            5: <Tag color="green" className="w-full text-center">Hoàn thành</Tag>,
                            6: <Tag color="green" className="w-full text-center">KT Xác nhận</Tag>,
                            7: <Tag color="green" className="w-full text-center">GĐ Xác nhận</Tag>,
                            8: <Tag color="tomato" className="w-full text-center">Mời lại</Tag>
                        },
                        width: 100,
                        search: false
                    },
                    {
                        title: 'Ghi chú',
                        dataIndex: 'note',
                        ellipsis: true
                    }
                ]}
                search={{
                    layout: 'vertical'
                }}
            />
        </>
    );
}

export default EventDashboard;