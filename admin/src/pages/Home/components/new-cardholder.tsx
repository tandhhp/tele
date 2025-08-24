import { apiListNewCardHolder } from "@/services/user";
import { ManOutlined, UserOutlined, WomanOutlined } from "@ant-design/icons";
import { ProTable } from "@ant-design/pro-components"
import { Tag } from "antd";
import dayjs from "dayjs";

const NewCardHolder: React.FC = () => {
    return (
        <ProTable
            request={apiListNewCardHolder}
            columns={[
                {
                    title: '#',
                    valueType: 'indexBorder',
                    width: 30
                },
                {
                    title: <UserOutlined />,
                    dataIndex: 'avatar',
                    valueType: 'avatar',
                    width: 30
                },
                {
                    title: 'Họ & tên',
                    dataIndex: 'name',
                    render: (dom, entity) => {
                        if (entity.gender) return (<><ManOutlined className="text-blue-500 mr-1" />{dom}</>);
                        return (<><WomanOutlined className="text-red-500 mr-1" />{dom}</>);
                    }
                },
                {
                    title: 'Ngày tham gia',
                    dataIndex: 'createdDate',
                    valueType: 'date',
                    render: (_, entity) => entity.createdDate ? dayjs(entity.createdDate).format('DD-MM-YYYY') : '-'
                },
                {
                    title: 'Hạng',
                    dataIndex: 'cardId',
                    render: (_, entity) => <Tag color={entity.tierColor} className='w-full text-center'>{entity.tierName}</Tag>,
                    width: 90
                }
            ]}
            className="mb-4"
            search={false}
            headerTitle="Chủ thẻ mới"
        />
    )
}

export default NewCardHolder;